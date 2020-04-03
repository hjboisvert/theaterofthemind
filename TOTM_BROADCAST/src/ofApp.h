#pragma once

#include <vector>
#include <utility>

#include "ofMain.h"
#include "video/ofGstVideoGrabber.h"
#include "video/ofGstUtils.h"
#include "gst/gst.h"
//#include "ofxGStreamer.h"
#include "ofxOsc.h"

// listening port
#define OSC_PORT 8000

class ofApp : public ofBaseApp {

	public:
		void setup();
		void update();
		void draw();
    void exit();

    void updateSenderStatus(string id, string status);
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

    GstElement *testSrc1;

		//ofGstVideoGrabber grabber;
		ofGstVideoUtils cam1;
		ofGstVideoUtils cam2;

    ofGstUtils gstUtils;
		//ofVideoGrabber grabber;
		ofTexture vidTexture1;
		ofTexture vidTexture2;

		int camWidth;
		int camHeight;

    ofxOscReceiver oscReceiver;
    vector<pair<string, bool>> statusVector;

    ofJson jsonConfig;
};
