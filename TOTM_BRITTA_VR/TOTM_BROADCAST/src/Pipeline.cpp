#include "Pipeline.h"

Pipeline::Pipeline(int previewWidth, int previewHeight) {
	frontSample = NULL;
	backSample = NULL;
	appsinkElement = NULL;
	pipeline = NULL;

	pixels.allocate(previewWidth, previewHeight, OF_PIXELS_RGBA);
	vidTexture.allocate(previewWidth, previewHeight, OF_PIXELS_RGBA);

	newFrame = false;
	isClosing = false;
	errorFlag = false;
	errorMessage = "";
}

void Pipeline::startGstMainLoop() {
	ofGstUtils::startGstMainLoop();
}

void Pipeline::setPipelineString(string pipelineStr) {
	pipelineString = pipelineStr;
}

void Pipeline::start() {
	ofLog() << "Starting gstreamer with pipeline: " << pipelineString << endl;

	GError* error = NULL;
	pipeline = gst_parse_launch(pipelineString.c_str(), &error);
	if (error) {
		errorFlag = true;
		errorMessage = string("Failed to parse pipeline: ") + error->message;
		ofLogError() << errorMessage;
		g_error_free(error);
		return;
	}

	// Find the appsink by name and attach callbacks directly,
	// bypassing ofGstUtils which crashes on GStreamer 1.24
	appsinkElement = gst_bin_get_by_name(GST_BIN(pipeline), "videoSink");
	if (appsinkElement) {
		ofLog() << "DEBUG: Found appsink, attaching raw GStreamer callbacks";
		GstAppSinkCallbacks callbacks = { 0 };
		callbacks.new_preroll = sOnNewPreroll;
		callbacks.new_sample = sOnNewSample;
		gst_app_sink_set_callbacks(GST_APP_SINK(appsinkElement), &callbacks, this, NULL);
		gst_app_sink_set_drop(GST_APP_SINK(appsinkElement), TRUE);
		gst_app_sink_set_max_buffers(GST_APP_SINK(appsinkElement), 2);
	} else {
		ofLog() << "No appsink found (preview disabled)";
	}

	// Set up bus message handler
	GstBus* bus = gst_element_get_bus(pipeline);
	gst_bus_add_watch(bus, sOnBusMessage, this);
	gst_object_unref(bus);

	// Start pipeline
	gst_element_set_state(pipeline, GST_STATE_PLAYING);
	newFrame = false;
}

void Pipeline::update() {
	if (isClosing || !pipeline) return;

	GstState state;
	gst_element_get_state(pipeline, &state, NULL, 0);
	if (state != GST_STATE_PLAYING && state != GST_STATE_PAUSED) return;

	mutex.lock();
	if (frontSample != backSample) {
		if (frontSample) gst_sample_unref(frontSample);
		frontSample = backSample;
		gst_sample_ref(frontSample);
		newFrame = true;
		mutex.unlock();

		GstBuffer* _buffer = gst_sample_get_buffer(frontSample);
		gst_buffer_map(_buffer, &mapinfo, GST_MAP_READ);
		pixels.setFromExternalPixels(mapinfo.data, pixels.getWidth(), pixels.getHeight(), pixels.getPixelFormat());
		gst_buffer_unmap(_buffer, &mapinfo);
	} else {
		newFrame = false;
		mutex.unlock();
	}

	if (newFrame) {
		vidTexture.loadData(pixels);
		newFrame = false;
	}
}

void Pipeline::draw(float x, float y) {
	vidTexture.draw(x, y, vidTexture.getWidth(), vidTexture.getHeight());
}

GstFlowReturn Pipeline::sOnNewPreroll(GstAppSink* sink, gpointer userData) {
	return GST_FLOW_OK;
}

GstFlowReturn Pipeline::sOnNewSample(GstAppSink* sink, gpointer userData) {
	Pipeline* self = static_cast<Pipeline*>(userData);

	GstSample* sample = gst_app_sink_pull_sample(GST_APP_SINK(sink));
	if (!sample) return GST_FLOW_ERROR;

	self->mutex.lock();
	GstSample* old = self->backSample;
	self->backSample = sample;
	self->mutex.unlock();

	if (old) gst_sample_unref(old);
	return GST_FLOW_OK;
}

gboolean Pipeline::sOnBusMessage(GstBus* bus, GstMessage* msg, gpointer userData) {
	Pipeline* self = static_cast<Pipeline*>(userData);
	self->onBusMessage(msg);
	return TRUE;
}

bool Pipeline::onBusMessage(GstMessage* msg) {
	switch (GST_MESSAGE_TYPE(msg)) {
	case GST_MESSAGE_ELEMENT: {
		ofLogVerbose() << "Got " << GST_MESSAGE_TYPE_NAME(msg) << " message from " << GST_MESSAGE_SRC_NAME(msg);
		ofLogVerbose() << gst_structure_to_string(gst_message_get_structure(msg));
		break;
	}
	case GST_MESSAGE_EOS: {
		ofLogVerbose() << "Got EOS" << endl;
		isClosing = true;
		gst_element_set_state(pipeline, GST_STATE_NULL);
		break;
	}
	case GST_MESSAGE_QOS: {
		GstFormat format;
		guint64 processed;
		guint64 dropped;
		gst_message_parse_qos_stats(msg, &format, &processed, &dropped);
		ofLogVerbose() << "QOS: format " << gst_format_get_name(format) << " processed " << processed << " dropped " << dropped;
		break;
	}
	case GST_MESSAGE_ERROR: {
		errorFlag = true;
		isClosing = true;
		GError* err = NULL;
		gchar* dbg_info = NULL;
		ofLogError() << "Got Error MSG";
		gst_message_parse_error(msg, &err, &dbg_info);
		ostringstream errorStream;
		errorStream << "Error from element " << GST_OBJECT_NAME(msg->src) << ": " << err->message;
		errorMessage = errorStream.str();
		ofLogError() << errorMessage;
		ofLogError() << "Debugging info: " << ((dbg_info) ? dbg_info : "none");
		g_error_free(err);
		g_free(dbg_info);
		gst_element_set_state(pipeline, GST_STATE_NULL);
		break;
	}
	case GST_MESSAGE_WARNING: {
		GError* err;
		gchar* debug;
		gst_message_parse_warning(msg, &err, &debug);
		ofLogWarning() << "Got Warning MSG from element " << GST_OBJECT_NAME(msg->src) << ": " << err->message;
		ofLogWarning() << "Debugging info: " << ((debug) ? debug : "none");
		g_error_free(err);
		g_free(debug);
		break;
	}
	case GST_MESSAGE_INFO: {
		GError* err;
		gchar* debug;
		gst_message_parse_info(msg, &err, &debug);
		ofLogVerbose() << "Got Info MSG from element " << GST_OBJECT_NAME(msg->src) << ": " << err->message;
		ofLogVerbose() << "Debugging info: " << ((debug) ? debug : "none");
		g_error_free(err);
		g_free(debug);
		break;
	}
	default:
		break;
	}
	return false;
}

string Pipeline::getErrorMessage() {
	return errorMessage;
}

void Pipeline::close() {
	if (isClosing) return;
	isClosing = true;

	if (pipeline) {
		ofLog() << "Sending EOS to pipeline" << endl;
		gboolean res = gst_element_send_event(pipeline, gst_event_new_eos());
		if (!res) {
			ofLogError() << "ERR could not send EOS" << endl;
		}
		ofLog() << "Waiting for pipeline to shut down" << endl;
		ofSleepMillis(500);
		gst_element_set_state(pipeline, GST_STATE_NULL);
		gst_object_unref(pipeline);
		pipeline = NULL;
	}

	if (appsinkElement) {
		gst_object_unref(appsinkElement);
		appsinkElement = NULL;
	}

	ofLog() << "Pipeline closed" << endl;
}

Pipeline::~Pipeline() {
	close();
}
