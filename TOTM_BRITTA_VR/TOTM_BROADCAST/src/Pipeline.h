#pragma once
#include "ofMain.h"
#include "ofGstUtils.h"
class Pipeline : public ofGstAppSink {
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
	GstElement * buildPipeline(string name);

	// ofGstAppSink overrides
	bool on_message(GstMessage* msg);
	GstFlowReturn on_preroll(shared_ptr<GstSample> buffer);
	GstFlowReturn on_buffer(shared_ptr<GstSample> buffer);

	ofGstUtils gstUtils;

	GstElement *pipeline;
	GstSample *frontSample, *backSample;
	GstMapInfo mapinfo;
	ofMutex mutex;
	ofPixels pixels;
	ofTexture vidTexture;

	string pipelineString;
	bool newFrame;

	string errorMessage;
};
