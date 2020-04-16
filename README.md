# theaterofthemind
Theater of the Mind is a hybrid experience of theater, performance art and art exhibition, based on the work of neuroscience labs around the world. Intimate audience groups of 16 go on a journey through a series of rooms with a guide, whose unfolding memories of a life lived backwards steers the investigation. Together, we are the subject of the work. The experience is visceral, emotional, and engaging, and from this journey comes a newfound sense of interdependence. We believe that Theater of the Mind is relevant and compelling because it offers audiences the opportunity to re-examine their assumptions about reality through first-hand encounters that demonstrate the individual, contextual, biased and malleable nature of perception. If we can change how we understand we see the world, what else can we change?


## TOTM_VR
Unity VR app that displays stereoscopic dome projection from two live wide-angle video streams

#### Hardware
 * [Oculus Rift S](https://www.oculus.com/rift-s/?locale=en_US)

#### Software
 * [Unity 2019.3.7f1](https://store.unity.com/download?ref=personal)
 * [Oculus Rift Software](https://www.oculus.com/rift/setup/?locale=en_US)
 * [Gstreamer v1.16.2](https://gstreamer.freedesktop.org/download/) (runtime and development files)

## TOTM_BROADCAST
openframeworks app that receives live video from two cameras wired to the PC, live audio from a microphone, and then broadcasts to local network over RTP

#### Hardware
 * 2x [Kodak Pixpro SP360 4K](https://kodakpixpro.com/cameras/360-vr/sp360-4k)
 * 2x [Elgato Cam Link 4K](https://www.elgato.com/en/gaming/cam-link-4k)

#### Software
 * [Openframeworks v0.11.0](https://openframeworks.cc/download/) for Visual Studio
   * with [gstreamer addon](https://github.com/arturoc/ofxGStreamer)
 * [Visual Studio Community 2017](https://visualstudio.microsoft.com/vs/older-downloads/)
   * Openframeworks addon

## Comms / Protocols

* **Media**
  * Audio and Video are streamed from Broadcast to VR PCs using RTP over the network broadcast address
* **Control**
  * Controls are sent from Broadcast to VR PCs using OSC
    * `/recenter` tells all VR apps to recenter the view when users are facing forward
    * `/fadeout` and `/fadein` (work in progress) will trigger a fade to or from black within VR respectively
  * Additionally the VR PCs periodically send a `/heartbeat` message back to Broadcast to give their status

## Development Install Guide
Both apps are developed and run on Windows 10.

**Gstreamer**

Required for both apps

1. Go to the [download page](https://gstreamer.freedesktop.org/download/) and download both the runtime and development installers under MinGW 64-bit
2. Install each one, making sure to select **COMPLETE** install when prompted. Leave the default install location as `C:\`
3. Add gstreamer binary folder path to System Environment Variables:
	1. Open Settings from the Start menu and type 'environment' in the search area
	2. Click the top result and then click  `Environment Variables...` on the bottom of the window that opens.
	3. Select `Path` in the list of variables and then click `Edit`
	4. In the new window, click `New` and then click 'Browse...`
	5. Go to `This PC` --> `Windows(C:)`-->`gstreamer`-->`1.0`-->`x86_64` and select `bin`, then click `OK`
	6. The new entry should read `C:\gstreamer\1.0\x86_64\bin`
	7. Click OK on the three windows you opened
	8. To see that the install was successful, open a command prompt and try `gst-launch-1.0 -h`

#### TOTM_BROADCAST

**openFrameworks** 

1. Download the Visual Studio version of openFrameworks from [this page](https://openframeworks.cc/setup/vs/) and extract. I put it under `Documents` but exact location is not important.
2. Install Visual Studio Community 2017 and the openFrameworks plugin using the instructions on [this page](https://openframeworks.cc/setup/vs/)
3. Copy the `TOTM_BROADCAST` folder to ` of_v0.11.0_vs2017_release\apps\myApps\`
4.  Double-click `TOTM_BROADCAST.sln` to open in Visual Studio

**elgato**

1. Download and install 4K Capture Utility for Windows from [this page](https://www.elgato.com/en/gaming/downloads) 
2. Go with defaults and click `Finish` 


#### TOTM_VR

**UNITY**
1. Download and install from https://store.unity.com/download?ref=personal 
2. Click the accept terms box to download unity hub
3. install to default location
4. click finish to open Unity Hub
5. Dismiss warnings about license
6. Go to main menu and click Installs
7. choose 2019.3.3f1 [the latest as of 03/04/2020] and go with defaults
8. Before you can use it, you need to log in with a free personal Unity account

**Oculus** 

1. Download and install from https://www.oculus.com/setup/ 
2. install to default location
3. connect oculus to HDMI to port in rear
4. Sign in to oculus
5. Start setup (requires one oculus controller)
6. Install firmware update(s) (replugging headset may be needed after an update)
7. put headset on and set floor level (you can ignore defining ‘play area’)

#### Test Commands
Use these to check that the cameras, gstreamer, the elgato devices, and the network are all working.

**On Broadcast PC** (two separate terminals)
*  `gst-launch-1.0 -v ksvideosrc device-index=1 ! video/x-raw, framerate=(fraction)30/1  ! queue ! x264enc tune=zerolatency speed-preset=ultrafast pass=cbr bitrate=16384 ! rtph264pay ! queue min-threshold-time=100000000 max-size-bytes=0 max-size-buffers=0 max-size-time=0 ! udpsink host=192.168.0.255 port=5000`
*   `gst-launch-1.0 -v ksvideosrc device-index=0 ! video/x-raw, framerate=(fraction)30/1  ! queue ! x264enc tune=zerolatency speed-preset=ultrafast pass=cbr bitrate=16384 ! rtph264pay ! queue min-threshold-time=100000000 max-size-bytes=0 max-size-buffers=0 max-size-time=0 ! udpsink host=192.168.0.255 port=5005`

**NOTE**: device-indices above may not be 0 and 1 for the cameras. Run `gst-device-monitor-1.0` to find the correct numbers if the above commands fail

**On VR PC** (two separate terminals)
* `gst-launch-1.0 -v udpsrc port=5000 ! application/x-rtp ! rtpjitterbuffer ! queue ! rtph264depay ! h264parse ! avdec_h264 ! videoconvert ! video/x-raw, format=I420, framerate=30/1 ! autovideosink`
* `gst-launch-1.0 -v udpsrc port=5005 ! application/x-rtp ! rtpjitterbuffer ! queue ! rtph264depay ! h264parse ! avdec_h264 ! videoconvert ! video/x-raw, format=I420, framerate=30/1 ! autovideosink`

## Parameters / Configuration

Things we will likely want to alter or tweak in the future

* **Gstreamer broadcast pipeline**
  * The gstreamer pipeline is parameterized in [config.json](TOTM_BROADCAST/bin/data/config.json)
  * The main option string we may want to change is `video_encoding`.
    * More on video encoding options in [this doc](Docs/h264_encoding.md)
* **Fisheye projection**
  * The algorithm for projecting the fisheye camera frames onto the sphere geometry is implemented in [PixPro4kFishEye.shader](TOTM_VR/Assets/Resources/Shaders/PixPro4kFishEye.shader)
    * More on fisheye projection in [this doc](Docs/fisheye_projection.md)
* **Periphery mask**
  * The mask for blocking the cameras from one another is an image file asset that is used by the fisheye shader. We may want something other than the black mask currently being used.
    * To use a different mask file, we can drop the new file into `Assets`-->`Resources`-->`Masks` and then Assigning the new mask to the Mask property of `Assets/Resources/SphereLeftMaterial` and `Assets/Resources/SphereRightMaterial` in the editor. 
    * To change what the shader *does* with the masked area we would edit [PixPro4kFishEye.shader](TOTM_VR/Assets/Resources/Shaders/PixPro4kFishEye.shader) 



## Future Improvements

* **User cues for recentering process**
  * We will want something to display to the users when they first put on the headsets to let them know when to face forward to recenter. HUD of some kind?
* **Fade to black**
  * working on this in the 'fadeout' branch

