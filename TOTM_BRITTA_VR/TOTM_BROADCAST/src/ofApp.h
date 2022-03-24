#pragma once

#include <vector>
#include <map>
#include <utility>
#include <regex>

#include "ofMain.h"
#include "video/ofGstVideoGrabber.h"
#include "video/ofGstUtils.h"
#include "gst/gst.h"
//#include "ofxGStreamer.h"
#include "ofxOsc.h"

#include "Pipeline.h"
#include "DeviceEnumerator.h";

// name of config file in "/bin/data"
#define CONFIG_FILENAME "config.json"

// listening port
#define OSC_PORT 9000

typedef tuple<string, bool, bool, int> StatusInfo;

class ofApp : public ofBaseApp {

  public:
		void setup();
		void update();
		void draw();
		void exit();

		void startNewPipeline();
		string buildPipelineString(string configName);

		void drawReceiverStatusList();
		void drawKeyControlGuide();
		void drawErrorMessage(string msg);
		void drawRetryMessage(string msg);
		void updateReceiverStatus(int id, int status);
		void checkReceiverConnections(int t);
		void keyPressed(int key);
		void keyReleased(int key);
		void mouseMoved(int x, int y );
		void mouseDragged(int x, int y, int button);
		void mousePressed(int x, int y, int button);
		void mouseReleased(int x, int y, int button);
		void mouseEntered(int x, int y);
		void mouseExited(int x, int y);
		void windowResized(int w, int h);
		void dragEvent(ofDragInfo dragInfo);
		void gotMessage(ofMessage msg);
		void sendRecenter();
		void handleFadeKey();
		void handleBlinkKey();
		void handleSwitchCams();
		void sendFadeout();
		void sendFadein();
		void sendBlinkOpen();
		void sendBlinkClosed();

		string broadcastAddr;
		shared_ptr<Pipeline> pipeline;

		ofTrueTypeFont font;

		int camWidth;
		int camHeight;
		int leftCamIndex;
		int rightCamIndex;

		string audioDeviceId;

		int previewWidth;
		int previewHeight;

		ofxOscReceiver oscReceiver;
		ofxOscSender oscSender;

		vector<StatusInfo> statusVector;
		int lastConnectionCheck;

		bool bRetryPending = false;
		int pipelineErrorTime;

		ofJson jsonConfig;
		string pipelineConfigName;

		// OSC addresses to listen on. populated from config file
		std::map<string, string> oscCommands = {
			{"recenter", ""},
			{"fadein", ""},
			{"fadeout", ""},
			{"blinkopen", ""},
			{"blinkclosed", ""},
			{"swapcameras", ""}
		};

		bool bFadeout = false;
		bool bBlinkClosed = false;
	
		bool bNeedNewPipeline = true;
		bool bConfigLoaded = false;
		string errorMessage;
		bool bShowErrorMessage = false;

		string retryMessage;
		bool bShowRetryMessage = false;

		ofGstUtils gstUtils;
		shared_ptr<GstDeviceMonitor> deviceMonitor;
};
