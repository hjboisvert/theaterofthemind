#include "DeviceEnumerator.h"

std::string DeviceEnumerator::getAudioDeviceId(std::string targetDeviceApi, std::string targetDeviceName) {
	std::string deviceIdString("");

	GstDeviceMonitor *deviceMonitor;
	GstCaps * caps;

	deviceMonitor = gst_device_monitor_new();

	caps = gst_caps_new_empty_simple("audio/x-raw");
	gst_device_monitor_add_filter(deviceMonitor, "Audio/Source", caps);
	gst_caps_unref(caps);

	caps = gst_caps_new_empty_simple("video/x-raw");
	gst_device_monitor_add_filter(deviceMonitor, "Video/Source", caps);
	gst_caps_unref(caps);

	if (!gst_device_monitor_start(deviceMonitor)) {
		ofLog() << "ERROR: COULD NOT START DEVICE MONITOR" << std::endl;
		return "";
	}

	GList *devices = gst_device_monitor_get_devices(deviceMonitor);
	if (g_list_length(devices) == 0) {
		ofLog() << "DEVICE MONITOR -- NO DEVICES FOUND" << std::endl;
	}
	GstDevice *device;
	gchar *name;

	ofLog() << "TARGET AUDIO API: " << targetDeviceApi << std::endl;
	ofLog() << "TARGET AUDIO DEVICE: " << targetDeviceName << std::endl;
	int i = 0;
	while (g_list_nth_data(devices, i) != NULL) {
		device = (GstDevice *)g_list_nth_data(devices, i);
		name = gst_device_get_display_name(device);
		if (targetDeviceName.compare(std::string(name)) != 0) {
			i++;
			continue;
		}
		ofLog() << "FOUND DEVICE: " << name << endl;
		GstStructure *props = gst_device_get_properties(device);
		gchar *propString = gst_structure_to_string(props);
		if (propString) {
			ofLog() << "" << propString << endl;
		}

		if (gst_structure_has_field(props, "device.api")) {
			const gchar *api = gst_structure_get_string(props, "device.api");
			if (targetDeviceApi.compare(std::string(api)) != 0) {
				i++;
				continue;
			}
            if (gst_structure_has_field(props, "device.strid")) {
                const gchar *strid = gst_structure_get_string(props, "device.strid");
                ofLog() << "FOUND REALTEK DEVICE: " << strid << std::endl;
                std::string stridStr(strid);
				std::string strEscaped = stridStr;
				strEscaped = std::regex_replace(strEscaped, std::regex("(\\{)"), "\\\\{");
				strEscaped = std::regex_replace(strEscaped, std::regex("(\\})"), "\\\\}");

                ofLog() << "ESCAPED DEVICE ID STR: " << strEscaped << std::endl;
                deviceIdString = strEscaped;
            }
		}

		g_free(name);
		gst_object_unref(device);
		i++;
	}

	g_list_free(devices);
	gst_device_monitor_stop(deviceMonitor);

	gst_object_unref(deviceMonitor);
	return deviceIdString;
}
