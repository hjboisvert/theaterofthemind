#pragma once
#include "ofMain.h"
#include "ofGstUtils.h"
#include <gst/app/gstappsink.h>

class Pipeline {
public:
	Pipeline(int previewWidth, int previewHeight);
	~Pipeline();
	void setPipelineString(string pipelineStr);
	static void startGstMainLoop();
	void start();
	void update();
	void draw(float x, float y);
	string getErrorMessage();
	void close();
	bool errorFlag;
	bool isClosing;

private:
	// Static GStreamer callbacks (raw API, bypasses ofGstUtils)
	static GstFlowReturn sOnNewPreroll(GstAppSink* sink, gpointer userData);
	static GstFlowReturn sOnNewSample(GstAppSink* sink, gpointer userData);
	static gboolean sOnBusMessage(GstBus* bus, GstMessage* msg, gpointer userData);

	bool onBusMessage(GstMessage* msg);

	GstElement* pipeline;
	GstElement* appsinkElement;
	GstSample* frontSample;
	GstSample* backSample;
	GstMapInfo mapinfo;
	ofMutex mutex;
	ofPixels pixels;
	ofTexture vidTexture;

	string pipelineString;
	bool newFrame;
	string errorMessage;
};
