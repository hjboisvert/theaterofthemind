#include "ofApp.h"

//--------------------------------------------------------------
void ofApp::setup(){
    /* testing gst lib */
    int argc = 0;
    char** argv = {0};
    gst_init(&argc, &argv);
    testSrc1 = gst_element_factory_make("videotestsrc", "testsrc1");
    if (!testSrc1) {
        ofLogError() << "Failed to create testsrc1";
    }
    /* */

    ofLog() << "Starting OSC listener on port " << to_string(OSC_PORT);
    oscReceiver.setup(OSC_PORT);

    int DEFAULT_WIDTH = 3840;
    int DEFAULT_HEIGHT = 2160;
    float DEFAULT_SCALE = 1.0;

    int rawWidth = DEFAULT_WIDTH;
    int rawHeight = DEFAULT_HEIGHT;
    float scale = DEFAULT_SCALE;

    string videoSrc1 = "ksvideosrc device-index=1";
    string videoSrc2 = "ksvideosrc device-index=3";
    string audioSrc = "directSoundSrc";

    string ipAddr = "192.168.0.255";

    string videoEncoding = "x264enc tune=zerolatency bitrate=16384 cabac=true dct8x8=true speed-preset=veryfast ! video/x-h264, profile=high";
    string audioEncoding = "volume volume=0.5 ! audioconvert ! audio/x-raw, rate=48000, channels=2 ! opusenc";

    ofFile file("config.json");
    if (file.exists()) {
        file >> jsonConfig;

        // size of frames coming from each camera
        rawWidth = jsonConfig[0]["raw_width"];
        rawHeight = jsonConfig[0]["raw_height"];

        scale = jsonConfig[0]["scale_factor"];

        videoSrc1 = jsonConfig[0]["video_src_1"];
        videoSrc2 = jsonConfig[0]["video_src_2"];
        audioSrc = jsonConfig[0]["audio_src"];

        ipAddr = jsonConfig[0]["ip_address"];

        //videoSink1 = jsonConfig[0]["video_sink_1"];
        //videoSink2 = jsonConfig[0]["video_sink_2"];
        //audioSink = jsonConfig[0]["audio_sink"];

        string videoEncoding = jsonConfig[0]["video_encoding"];
        string audioEncoding = jsonConfig[0]["audio_encoding"];
    }

    string videoSink1 = "rtph264pay ! queue ! udpsink host=" + ipAddr + " port=5000";
    string videoSink2 = "rtph264pay ! queue ! udpsink host=" + ipAddr + " port=5005";
    string audioSink = "rtpopuspay ! udpsink host=" + ipAddr + " port=5001";


    string videoCaps = "video/x-raw, width=" + to_string(static_cast<int>(scale * rawWidth)) + ", height=" +
        to_string(static_cast<int>(scale * rawHeight)) + ", format=I420, framerate=(fraction)30/1";

    ofLog() <<  "videoCaps = " << videoCaps;

    float t_scale = 0.2;
    string t_element = " ! queue ! videoscale ! video/x-raw, width=" + to_string(static_cast<int>(t_scale * rawWidth)) + ", height=" +
        to_string(static_cast<int>(t_scale * rawHeight)) + ", format=I420, framerate=(fraction)30/1" + " ! videoconvert ! autovideosink";

    string pipelineString = videoSrc1 + " ! tee name=t1  ! queue ! videoscale ! " + videoCaps + " ! " + videoEncoding + " ! " + videoSink1 + " " +
        "t1." + t_element + " " +
        videoSrc2 + " ! tee name=t2 ! queue ! videoscale ! " + videoCaps + " ! " + videoEncoding + " ! " + videoSink2 + " " +
        "t2." + t_element + " " +
        audioSrc + " ! queue ! " + audioEncoding + " ! " + audioSink;


    gstUtils.startGstMainLoop();

    gstUtils.setPipelineWithSink(pipelineString, "", true);

	ofVideoGrabber tmpGrabber = ofVideoGrabber();
	vector<ofVideoDevice> devices = tmpGrabber.listDevices();
	if (devices.size() == 0) {
		cout << "NO DEVICES";
		ofLogError("setup") << "NO DEVICES";
	}

	for (size_t i = 0; i < devices.size(); i++) {
		if (devices[i].bAvailable) {
			//log the device
			ofLogNotice() << devices[i].id << ": " << devices[i].deviceName;
		}
		else {
			//log the device and note it as unavailable
			ofLogNotice() << devices[i].id << ": " << devices[i].deviceName << " - unavailable ";
		}
	}

	
    vidTexture1.allocate(camWidth, camHeight, OF_PIXELS_RGB);
    vidTexture1.allocate(camWidth, camHeight, OF_PIXELS_RGB);

    ofSetFrameRate(60);
    ofSetVerticalSync(true);

    gstUtils.startPipeline();
    gstUtils.play();
}

//--------------------------------------------------------------
void ofApp::update(){
    ofBackground(100, 100, 100);
    if (!gstUtils.isPlaying() /*|| !cam2.isInitialized()*/) {
        ofLogNotice() << "NOT PLAYING";
        return;
    }

    // for OSC, this app is receiver and the VR PCs are senders
    while (oscReceiver.hasWaitingMessages()) {
        ofxOscMessage m;
        oscReceiver.getNextMessage(m);

        if (m.getAddress() == "/heartbeat") {
            ofLog() << "* Got heartbeat message *";
            string senderId = m.getArgAsString(0);
            ofLog() << "\tID: " << senderId;
            string senderStatus = m.getArgAsString(1);
            ofLog() << "\tStatus: " << senderStatus;

            updateSenderStatus(senderId, senderStatus);
        }
    }
}

void ofApp::updateSenderStatus(string id, string status) {
    int index = -1;
    for (int i = 0; i < statusVector.size(); i++) {
        pair<string, int> statusPair = statusVector.at(i);
        if(statusPair.first == id) {
            index = i;
            break;
        }
    }
    bool bStatus = status == "true" ? true : false;
    if (index >= 0) {
        statusVector.at(index).second = bStatus;
    } else {
        statusVector.emplace_back(id, bStatus);
    }
}

//--------------------------------------------------------------
void ofApp::draw(){
	ofBackground(0x0);
	//vidTexture1.draw(0, 0, ofGetWidth()/2, ofGetHeight());
	//vidTexture2.draw(ofGetWidth()/2, 0, ofGetWidth()/2, ofGetHeight());
  int statusListX = 20;
  int statusListY = 20;
  int incrY = 20;
  int index = 0;
  for (pair<string, bool> status : statusVector) {
      string bStr;
      if (status.second) {
          bStr = "OK";
          ofSetColor(ofColor::green);
      } else {
          bStr = "DOWN";
          ofSetColor(ofColor::red);
      }
      string statusString = status.first + ": " + bStr;
      ofDrawBitmapString(statusString, statusListX, statusListY + index*incrY);
      index++;
  }
}

//--------------------------------------------------------------
void ofApp::exit() {

}

//--------------------------------------------------------------
void ofApp::keyPressed(int key){

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
