#include "ofApp.h"

//--------------------------------------------------------------
void ofApp::setup(){
	ofLog() << "starting setup" << endl;
	ofSetLogLevel(OF_LOG_NOTICE);

	ofSetWindowPosition(0,0);
	ofSetWindowShape(ofGetScreenWidth(), ofGetScreenHeight());

	font.load("C:\\Windows\\Fonts\\consola.ttf", 14);

	broadcastAddr = "127.0.0.1"; // fallback in case not set in config.json

	// find pipeline with this name in the config and build it
	pipelineConfigName = "gst_test_src";

    ofFile file(CONFIG_FILENAME);
    if (file.exists()) {
        file >> jsonConfig;
		file.close();

		// STREAM PIPELINE CONFIG
		pipelineConfigName = jsonConfig[0]["current_pipeline_name"].get<std::string>();
		broadcastAddr = jsonConfig[0]["broadcast_addr"].get<std::string>();
		leftCamIndex = jsonConfig[0]["cameras"]["left_camera_index"].get<int>();
		rightCamIndex = jsonConfig[0]["cameras"]["right_camera_index"].get<int>();

		// OSC RECEIVER CONFIG
		/* commands defined in google sheet 'ToTM Show Paperwork'*/
		oscCommands["recenter"] = jsonConfig[0]["osc_commands"]["recenter"].get<std::string>();
		oscCommands["fadein"] = jsonConfig[0]["osc_commands"]["fadein"].get<std::string>();
		oscCommands["fadeout"] = jsonConfig[0]["osc_commands"]["fadeout"].get<std::string>();
		oscCommands["blinkopen"] = jsonConfig[0]["osc_commands"]["blinkopen"].get<std::string>();
		oscCommands["blinkclosed"] = jsonConfig[0]["osc_commands"]["blinkclosed"].get<std::string>();
		oscCommands["swapcameras"] = jsonConfig[0]["osc_commands"]["swapcameras"].get<std::string>();

		bConfigLoaded = true;
    } else {
		bShowErrorMessage = true;
		ostringstream errStream;
		errStream << "[ERROR] CONFIG FILE NOT FOUND:\n\t" << ofFilePath::getCurrentExeDir()
			<< "data\\" << CONFIG_FILENAME;
		errorMessage = errStream.str();
		ofLogError() << errorMessage << endl;
	}

	ofLog() << "starting gstreamer main loop" << endl;
	ofGstUtils::startGstMainLoop();

	ofLog() << "Finding audio device ID" << endl;
	std::string targetApi = jsonConfig[0]["audio"]["device_api"].get<std::string>();
	std::string targetName = jsonConfig[0]["audio"]["device_display_name"].get<std::string>();
	jsonConfig[0]["audio"]["device_id"] = DeviceEnumerator::getAudioDeviceId(targetApi, targetName);

    ofLog() << "Starting OSC listener on port " << to_string(OSC_PORT);
    oscReceiver.setup(OSC_PORT);

    ofLog() << "Starting OSC sender to " << broadcastAddr << to_string(OSC_PORT);
    oscSender.setup(broadcastAddr, 8000);

	lastConnectionCheck = ofGetUnixTime();

	ofBackground(0);
    ofSetFrameRate(60);
    ofSetVerticalSync(true);

	previewWidth = (int)floor(ofGetWidth() * 0.8);
	previewHeight = (int)floor(previewWidth / 2);

	if (bConfigLoaded) {
		startNewPipeline();
	}
}


//--------------------------------------------------------------
void ofApp::startNewPipeline() {
	if (pipeline && !pipeline->errorFlag) {
		ofLog() << "Closing previous pipeline" << endl;
		pipeline->close();
	}

	string pipelineString = buildPipelineString(pipelineConfigName);
	if (pipelineString.length() > 0) {
		ofLog() << "Creating new pipeline" << endl;
		pipeline = make_shared<Pipeline>(previewWidth, previewHeight);
		pipeline->setPipelineString(pipelineString);
		ofLog() << "Starting pipeline" << endl;
		pipeline->start();
	}

	bNeedNewPipeline = false;
	bRetryPending = false;
}

//--------------------------------------------------------------
string ofApp::buildPipelineString(string pipelineConfigName) {
   string pipelineString = "";

   //int camBlockSize = 3840 * 2160 * 3 / 2; // number of bytes for 1 frame in NV12 format
   int camBlockSize = 4096;

   bool bFoundPipelineConfig = false;
    for (auto& pipeConfig : jsonConfig[0]["pipelines"]) {
        if (pipeConfig["name"].get<std::string>() == pipelineConfigName) {
			bFoundPipelineConfig = true;
			ofLog() << "Starting pipeline config named " << pipelineConfigName << endl;
			pipelineString += "ksvideosrc device-index=" + to_string(leftCamIndex) + " blocksize=" + to_string(camBlockSize) + "  name=left  ";
			pipelineString += "ksvideosrc device-index=" + to_string(rightCamIndex) + " blocksize=" + to_string(camBlockSize) + "  name=right  ";

			for (auto& pipeStr : pipeConfig["pipeline_array"]) {
				pipelineString += pipeStr.get<std::string>();
			}
			std::regex ipRegex("(\\bhost=\\d{1,3}\.\\d{1,3}\.\\d{1,3}\.\\d{1,3}\\b)", regex::ECMAScript); // regex to match "host=X.X.X.X" where X's can be 1-3 digits
			pipelineString = regex_replace(pipelineString, ipRegex, "host=" + broadcastAddr);

			string audioDeviceId = jsonConfig[0]["audio"]["device_id"].get<std::string>();
			pipelineString.replace(pipelineString.find("<audio_device_id>"), string("<audio_device_id>").length(), "device=\\\"" + audioDeviceId + "\\\"");

			if (pipeConfig["show_preview"].get<bool>()) {
				pipelineString += "glcolorscale ! videoconvert ! videoscale !  video/x-raw(memory:GLMemory), format=RGBA, width=" +
					to_string(previewWidth) + ", height=" + to_string(previewHeight) + ",framerate=(fraction)30/1 ! appsink name=videoSink sync=false";
			}
        }
    }

	if (!bFoundPipelineConfig) {
		bShowErrorMessage = true;
		ostringstream errStream;
		errStream << "[ERROR] PIPELINE CONFIG NOT FOUND IN " << CONFIG_FILENAME
			<< ":\n\t" << pipelineConfigName;
		errorMessage = errStream.str();
		ofLogError() << errorMessage << endl;
	}

	return pipelineString;
}

//--------------------------------------------------------------
void ofApp::update(){
	if (bConfigLoaded && bNeedNewPipeline) {
		ofLog() << "NEED NEW PIPELINE" << endl;
		startNewPipeline();
		return;
	}

	if (pipeline && pipeline->errorFlag) {
		if (!bShowErrorMessage) {
			bShowErrorMessage = true;
			ostringstream errStream;
			errStream << "[ERROR] PIPELINE FAILED:\n\t" << pipeline->getErrorMessage();
			errorMessage = errStream.str();
			ofLogError() << errorMessage << endl;
		}
		if (!bRetryPending) {
			bRetryPending = true;
			pipelineErrorTime = ofGetUnixTime();
		}
	} else if (pipeline) {
		pipeline->update();
	}

    // for OSC, this app is receiver and the VR PCs are senders
    while (oscReceiver.hasWaitingMessages()) {
        ofxOscMessage m;
        oscReceiver.getNextMessage(m);

        if (m.getAddress() == "/heartbeat") {
            ofLogVerbose() << "* Got heartbeat message *";
            int senderId = m.getArgAsInt(0);
            ofLogVerbose() << "\tID: " << senderId;
            int senderStatus = m.getArgAsInt(1);
            ofLogVerbose() << "\tStatus: " << senderStatus;

            updateReceiverStatus(senderId, senderStatus);
        }

		if (m.getAddress() == oscCommands["recenter"]) {
			sendRecenter();
		} else if (m.getAddress() == oscCommands["fadein"]) {
			sendFadein();
		} else if (m.getAddress() == oscCommands["fadeout"]) {
			sendFadeout();
		} else if (m.getAddress() == oscCommands["blinkopen"]) {
			sendBlinkOpen();
		} else if (m.getAddress() == oscCommands["blinkclosed"]) {
			sendBlinkClosed();
		} else if (m.getAddress() == oscCommands["swapcameras"]) {
			handleSwitchCams();
		}
    }

	int t = ofGetUnixTime();
	if ((t - lastConnectionCheck) >= 3) {
		checkReceiverConnections(t);
		lastConnectionCheck = t;
	}

	// If pipeline failed previously, wait 10 sec and try again
	if (bRetryPending) {
		int retryInterval = 10;
		int timeSinceError = t - pipelineErrorTime;
		if (timeSinceError >= 10) {
			bNeedNewPipeline = true;
			bRetryPending = false;
			bShowErrorMessage = false;
		} else {
			bShowRetryMessage = true;
			retryMessage = "Retrying in " + to_string(retryInterval - timeSinceError);
		}
	}
}

//--------------------------------------------------------------
void ofApp::updateReceiverStatus(int id, int status) {
	bool bSendFadeState = false; // whether we need to broadcast fade state
    int index = -1;
    for (int i = 0; i < statusVector.size(); i++) {
        StatusInfo status = statusVector.at(i);
        if(get<0>(status) == to_string(id)) {
            index = i;
            break;
        }
    }
	bool bUpStatus = true;
    bool bRecvStatus = status == 1 ? true : false;
    if (index >= 0) { // known client
		if (!get<1>(statusVector[index])) { // this client is back after going down
			bSendFadeState = true;
		}
		get<1>(statusVector[index]) = bUpStatus;
        get<2>(statusVector[index]) = bRecvStatus;
		get<3>(statusVector[index]) = ofGetUnixTime();
    } else { // first heartbeat from this client
		bSendFadeState = true;
        statusVector.emplace_back(to_string(id), bUpStatus, bRecvStatus, ofGetUnixTime());
		// keep sorted
		sort(statusVector.begin(), statusVector.end(), [](StatusInfo &s1, StatusInfo &s2) {
			// convert to int, otherwise 11 comes before 2
			return (stoi(get<0>(s1)) - stoi(get<0>(s2))) < 0;
		});
    }

	if (bSendFadeState) {
		sendFadeState();
	}
}

//--------------------------------------------------------------
void ofApp::sendFadeState() {
	// broadcast current fade state whenever there is new client, incl ones that have gone down and back up
	if (bFadeout) {
		sendFadeout();
	} else {
		sendFadein();
	}
}

//--------------------------------------------------------------
void ofApp::checkReceiverConnections(int now) {
	for (int i = 0; i < statusVector.size(); i++) {
		if ((now - get<3>(statusVector[i])) >= 3) { // haven't gotten heartbeat in 3 sec
			//ofLog() << "Receiver DOWN: " << get<0>(statusVector[i]) << endl;
			get<1>(statusVector[i]) = false;
		}
	}
}

//--------------------------------------------------------------
void ofApp::draw() {
	if (pipeline && !pipeline->errorFlag) {
		pipeline->draw(ofGetWidth()*0.95 - previewWidth, ofGetHeight()/2 - previewHeight/2);
	}

	ofPushStyle();

	drawReceiverStatusList();

	if (bShowErrorMessage) {
		drawErrorMessage(errorMessage);
	}

	if (bShowRetryMessage) {
		drawRetryMessage(retryMessage);
		bShowRetryMessage = false;
	}

	drawKeyControlGuide();

	drawFadeState();

	ofPopStyle();
}

//--------------------------------------------------------------
void ofApp::drawReceiverStatusList() {
	int statusListX = 20;
	float incrY = font.getLineHeight();
	int statusListY = incrY*2;
	int index = 1;

	ofSetColor(ofColor::white);
	font.drawString("VR PCs", statusListX, statusListY);
	for (StatusInfo status : statusVector) {
		string statusString = get<0>(status) + " :: ";
		statusString += get<1>(status) ? "UP" : "DOWN";
		if (get<1>(status)) {
			statusString += " :: ";
			statusString += get<2>(status) ? "RECEIVING" : "NOT RECEIVING";
		}
		if (get<1>(status) && get<2>(status)) {
			ofSetColor(ofColor::green);
		}
		else {
			ofSetColor(ofColor::red);
		}

		font.drawString(statusString, statusListX, statusListY + index * incrY);
		index++;
	}
}

//--------------------------------------------------------------
void ofApp::drawKeyControlGuide() {
	string titleText = "Keyboard Controls";
	ofSetColor(ofColor::white);
	font.drawString(titleText, ofGetWidth()/2 - font.stringWidth(titleText)/2,
		ofGetHeight() * 0.05);

	string controlsText = "R: Recenter \tF: Fade \tB: Blink \tC: Swap Cameras";
	font.drawString(controlsText, ofGetWidth()/2 - font.stringWidth(controlsText)/2,
		ofGetHeight() * 0.08);
}

//--------------------------------------------------------------
void ofApp::drawFadeState() {
	string fadeText = "Fade Out: FALSE";

	if (bFadeout) {
		fadeText = "Fade Out: TRUE";
	}

	ofSetColor(ofColor::magenta);
	font.drawString(fadeText, ofGetWidth()/2 - font.stringWidth(fadeText)/2,
		ofGetHeight() - ofGetHeight() * 0.1);
}

//--------------------------------------------------------------
void ofApp::drawErrorMessage(string message) {
	ofSetColor(ofColor::red);
	font.drawString(message, ofGetWidth()/2 - font.stringWidth(message)/2,
		ofGetHeight()/2 - font.stringHeight(message)/2);
}

//--------------------------------------------------------------
void ofApp::drawRetryMessage(string message) {
	ofSetColor(ofColor::white);
	font.drawString(message, ofGetWidth()/2 - font.stringWidth(message)/2,
		font.getLineHeight()*2);
}

//--------------------------------------------------------------
void ofApp::exit() {
	if (bConfigLoaded) {
		ofLog() << "Saving config..." << endl;
		ofSavePrettyJson("config.json", jsonConfig);
	}
	/*
	if (deviceMonitor) {
		gst_device_monitor_stop(deviceMonitor.get());
	}*/

	if (pipeline) {
		ofLog() << "Closing pipeline..." << endl;
		pipeline->close();
	}

	ofLog() << "EXIT" << endl;
}

//--------------------------------------------------------------
void ofApp::sendRecenter() {
    ofLog() << "sending RECENTER cmd" << std::endl;
    ofxOscMessage m;
    m.setAddress("/cmd");
	m.addStringArg("recenter");

    oscSender.sendMessage(m);
}

//--------------------------------------------------------------
void ofApp::sendFadeout() {
	ofLog() << "sending FADEOUT cmd" << std::endl;
	ofxOscMessage m;
	m.setAddress("/cmd");
	m.addStringArg("fadeout");

	oscSender.sendMessage(m);
	bFadeout = true;
}

//--------------------------------------------------------------
void ofApp::sendFadein() {
	ofLog() << "sending FADEIN cmd" << std::endl;
	ofxOscMessage m;
	m.setAddress("/cmd");
	m.addStringArg("fadein");

	oscSender.sendMessage(m);
	bFadeout = false;
}

//--------------------------------------------------------------
void ofApp::sendBlinkClosed() {
	ofLog() << "sending BLINK CLOSED cmd" << std::endl;
	ofxOscMessage m;
	m.setAddress("/cmd");
	m.addStringArg("blinkclosed");

	oscSender.sendMessage(m);
	bBlinkClosed = true;
}

//--------------------------------------------------------------
void ofApp::sendBlinkOpen() {
	ofLog() << "sending BLINK OPEN cmd" << std::endl;
	ofxOscMessage m;
	m.setAddress("/cmd");
	m.addStringArg("blinkopen");

	oscSender.sendMessage(m);
	bBlinkClosed = false;
}

//--------------------------------------------------------------
void ofApp::handleFadeKey() {
	if (!bFadeout) {
		sendFadeout();
	}
	else {
		sendFadein();
	}
}

//--------------------------------------------------------------
void ofApp::handleBlinkKey() {
	if (!bBlinkClosed) {
		sendBlinkClosed();
	}
	else {
		sendBlinkOpen();
	}
}

//--------------------------------------------------------------
void ofApp::handleSwitchCams() {
	leftCamIndex = (leftCamIndex + 1) % 2;
	jsonConfig[0]["cameras"]["left_camera_index"] = leftCamIndex;
	rightCamIndex = (rightCamIndex + 1) % 2;
	jsonConfig[0]["cameras"]["right_camera_index"] = rightCamIndex;
	bNeedNewPipeline = true;
}

//--------------------------------------------------------------
void ofApp::keyPressed(int key){
    switch(key) {
        case 'r':
        case 'R':
            sendRecenter();
            break;
		case 'f':
		case 'F':
			handleFadeKey();
			break;
		case 'b':
		case 'B':
			handleBlinkKey();
			break;
		case 'c':
		case 'C':
			handleSwitchCams();
			break;
        default:
            break;
    }
}

//--------------------------------------------------------------
void ofApp::keyReleased(int key){

}

//--------------------------------------------------------------
void ofApp::mouseMoved(int x, int y ){

}

//--------------------------------------------------------------
void ofApp::mouseDragged(int x, int y, int button){

}

//--------------------------------------------------------------
void ofApp::mousePressed(int x, int y, int button){

}

//--------------------------------------------------------------
void ofApp::mouseReleased(int x, int y, int button){

}

//--------------------------------------------------------------
void ofApp::mouseEntered(int x, int y){

}

//--------------------------------------------------------------
void ofApp::mouseExited(int x, int y){

}

//--------------------------------------------------------------
void ofApp::windowResized(int w, int h){

}

//--------------------------------------------------------------
void ofApp::gotMessage(ofMessage msg){

}

//--------------------------------------------------------------
void ofApp::dragEvent(ofDragInfo dragInfo){

}
