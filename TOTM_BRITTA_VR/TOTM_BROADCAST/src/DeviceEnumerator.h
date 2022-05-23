#pragma once
#include <regex>
#include "ofMain.h"
#include "video/ofGstUtils.h"
#include "gst/gst.h"

class DeviceEnumerator {
public:
	static std::string getAudioDeviceId(std::string deviceApi, std::string deviceName);
};
