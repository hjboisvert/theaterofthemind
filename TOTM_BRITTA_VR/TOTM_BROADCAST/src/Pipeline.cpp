#include "Pipeline.h"

Pipeline::Pipeline(int previewWidth, int previewHeight) {
	frontSample = NULL;
	backSample = NULL;

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

GstElement * Pipeline::buildPipeline(string pipelineString) {
    ofLog() << "Starting gstreamer with pipeline: " << pipelineString << endl;

	gstUtils.setPipelineWithSink(pipelineString, "videoSink", true);
	gstUtils.setFrameByFrame(false);
	return gstUtils.getPipeline();
}

void Pipeline::start() {
	gstUtils.setSinkListener(this);
	pipeline = buildPipeline(pipelineString);
	gstUtils.startPipeline();
	gstUtils.play();
	newFrame = false;
}

void Pipeline::update() {
	if (!gstUtils.isPlaying() || isClosing) {
        return;
    }

	mutex.lock();
	if (frontSample != backSample) {
		if (frontSample) gst_sample_unref(frontSample);
		frontSample = backSample;
		gst_sample_ref(frontSample);
		newFrame = true;
		mutex.unlock();

		GstBuffer *_buffer = gst_sample_get_buffer(frontSample);
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

GstFlowReturn Pipeline::on_preroll(shared_ptr<GstSample> buffer) {
	gstUtils.play();
	return GST_FLOW_OK;
}


GstFlowReturn Pipeline::on_buffer(shared_ptr<GstSample> sample) {
	mutex.lock();
	backSample = sample.get();
	mutex.unlock();

	return GST_FLOW_OK;
}

bool Pipeline::on_message(GstMessage * msg) {
	/**
	 *	Copied from https://github.com/arturoc/ofxGstRTP/blob/master/src/ofxGstRTPServer.cpp
	 **/

	// read messages from the pipeline like dropped packages
	switch (GST_MESSAGE_TYPE(msg)) {
	case GST_MESSAGE_ELEMENT: {
		ofLogVerbose() << "Got " << GST_MESSAGE_TYPE_NAME(msg) << " message from " << GST_MESSAGE_SRC_NAME(msg);
		ofLogVerbose() << "Message source type: " << G_OBJECT_CLASS_NAME(G_OBJECT_GET_CLASS(GST_MESSAGE_SRC(msg)));
		ofLogVerbose() << "With structure name: " << gst_structure_get_name(gst_message_get_structure(msg));
		ofLogVerbose() << gst_structure_to_string(gst_message_get_structure(msg));
		ofLogVerbose() << std::endl;
		break;
	}

	case GST_MESSAGE_EOS: {
		ofLogVerbose() << "Got EOS" << endl;
		gstUtils.close();
		break;
	}
	case GST_MESSAGE_QOS: {
		// ofLogVerbose() << "Dropped Frame" << endl;

		GstObject * messageSrc = GST_MESSAGE_SRC(msg);
		ofLogVerbose() << "Got " << GST_MESSAGE_TYPE_NAME(msg) << " message from " << GST_MESSAGE_SRC_NAME(msg);
		ofLogVerbose() << "Message source type: " << G_OBJECT_CLASS_NAME(G_OBJECT_GET_CLASS(messageSrc));

		GstFormat format;
		guint64 processed;
		guint64 dropped;
		gst_message_parse_qos_stats(msg, &format, &processed, &dropped);
		ofLogVerbose() << "format " << gst_format_get_name(format) << " processed " << processed << " dropped " << dropped;

		gint64 jitter;
		gdouble proportion;
		gint quality;
		gst_message_parse_qos_values(msg, &jitter, &proportion, &quality);
		ofLogVerbose() << "jitter " << jitter << " proportion " << proportion << " quality " << quality;

		gboolean live;
		guint64 running_time;
		guint64 stream_time;
		guint64 timestamp;
		guint64 duration;
		gst_message_parse_qos(msg, &live, &running_time, &stream_time, &timestamp, &duration);
		ofLogVerbose() << "live stream " << live << " runninng_time " << running_time << " stream_time " << stream_time << " timestamp " << timestamp << " duration " << duration;
		ofLogVerbose() << std::endl;

		break;
	}

	case GST_MESSAGE_ERROR: {
		errorFlag = true;
		isClosing = true;
		// from https://gstreamer.freedesktop.org/documentation/gstreamer/gstmessage.html?gi-language=c#gst_message_parse_error
		GError *err = NULL;
		gchar *dbg_info = NULL;

		ofLogError() << " Got Error MSG";
		gst_message_parse_error(msg, &err, &dbg_info);
		ostringstream errorStream;
		errorStream << "Error from element " << GST_OBJECT_NAME(msg->src) << ": " << err->message;
		errorMessage = errorStream.str();
		ofLogError() << errorMessage;
		ofLogError() << "Debugging info: " << ((dbg_info) ? dbg_info : "none");
		g_error_free(err);
		g_free(dbg_info);
		ofLogError() << std::endl;
		gstUtils.close();
		break;
	}
	case GST_MESSAGE_WARNING: {
		GError *err;
		gchar *debug;

		gst_message_parse_warning(msg, &err, &debug);
		ofLogWarning() << "Got Warning MSG from element " << GST_OBJECT_NAME(msg->src) << ": " << err->message;
		ofLogWarning() << "Debugging info: " << ((debug) ? debug : "none");
		g_clear_error(&err);
		g_error_free(err);
		g_free(debug);
		ofLogWarning() << std::endl;
		break;
	}
	case GST_MESSAGE_INFO: {
		GError *err;
		gchar *debug;

		gst_message_parse_info(msg, &err, &debug);
		ofLogVerbose() << "Got Info MSG from element " << GST_OBJECT_NAME(msg->src) << ": " << err->message;
		ofLogVerbose() << "Debugging info: " << ((debug) ? debug : "none");
		g_clear_error(&err);
		g_error_free(err);
		g_free(debug);
		ofLogVerbose() << std::endl;
		break;
	}

	default: {
		// ofLog() << "Got " << GST_MESSAGE_TYPE_NAME(msg) << " message from " << GST_MESSAGE_SRC_NAME(msg);
		// return false;
		break;
	}
	} // end switch
	return false;
}

string Pipeline::getErrorMessage() {
	return errorMessage;
}

void Pipeline::close() {
	if (isClosing) {
		return;
	} else {
		isClosing = true;
		gboolean res = gst_element_send_event(pipeline, gst_event_new_eos());
		if (!res) {
			ofLogError() << "ERR could not send EOS" << endl;
		}

		ofLog() << "Waiting for gstreamer pipeline to shut down" << endl;
		// wait for for EOS to propagate through pipeline and gstUtils to clean up
		while(gstUtils.isLoaded()) {
			ofSleepMillis(200);
		}
		ofLog() << "Done" << endl;
	}
}

Pipeline::~Pipeline() {
	close();
}

