# TOTM
Receiver and sender for Theater of the Mind VR experience

The TOTM.zip file, containing the latest builds of the VR and BROADCAST apps as well as the installers for all dependencies,
is located in [this drive folder](https://drive.google.com/drive/folders/1eEnmkPB91Y5Vo2imYkLwD9W3qooDah3d?usp=sharing).

For on-location (not dev) installation and setup of the VR and BROADCAST PCs, see [this doc](Docs/totm_pc_setup.md) then follow the cable connections and startup sections below.

For usage and common issues, see
[VR Procedures](https://docs.google.com/document/d/1x5ep65TPM-KHYitz88LDsFJRX1JCE8SJcl05volPcPo/edit?usp=sharing)

For troubleshooting, see [this doc](Docs/totm_troubleshooting.md).

Both apps are developed and tested on Windows 10 version 21H2. **IMPORTANT:** Windows updates have caused previous builds of TOTM_VR
to stop working.
We are using the same PC model \[[Link](https://www.msi.com/Desktop/Trident-3)\] for both apps.
Detailed system info for each of the PCs can be found [here](Docs/systeminfo/)

## TOTM_VR
Unity VR app that displays stereoscopic dome projection from two live wide-angle video streams

#### Hardware
 * [Oculus Rift S](https://www.oculus.com/rift-s/?locale=en_US)

#### Software
 * Windows 10 version 21H2
 * [Unity 2021.3.1f1](https://store.unity.com/download?ref=personal)
 * [Oculus Rift Software v39.0.0.65.369](https://www.oculus.com/rift/setup/?locale=en_US)
 * [Gstreamer v1.18.6](https://gstreamer.freedesktop.org/download/) (runtime and development files)
 * [Unity Gstreamer Plugin, commit 37845d](https://github.com/mrayy/mrayGStreamerUnity/tree/37845d2bc874758f33350242a9c6755488ccc1d7)
 * [OSCJack for Unity v0.1.4](https://github.com/keijiro/OscJack/tree/v0.1.4)
 * [Unity Oculus Integration Package v39.0](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022#releases)
 * [Unity Eye Blink Effect Package v1.0](https://assetstore.unity.com/packages/tools/particles-effects/eye-blink-effect-fps-vr-61275#releases)
 * [Python v3.10.6](https://www.python.org/downloads/release/python-3106/)

## TOTM_BROADCAST
openframeworks app that receives live video from two cameras wired to the PC, live audio from a microphone, and then broadcasts to local network over RTP (and RTCP for receivers to get necessary media and stream info)

#### Hardware
 * 2x [Kodak Pixpro SP360 4K](https://kodakpixpro.com/cameras/360-vr/sp360-4k)
 * 2x [Elgato Cam Link 4K](https://www.elgato.com/en/gaming/cam-link-4k)

#### Software
 * Windows 10 version 21H2
 * [Openframeworks v0.10.1](https://openframeworks.cc/download/) for Visual Studio. (note: v0.11.0 has also been tested)
   * with [ofxGStreamer addon, commit fb1109](https://github.com/arturoc/ofxGStreamer/tree/fb1109f995168bcb1d673724d97ba7931772945d)
 * [Visual Studio Community 2019](https://visualstudio.microsoft.com/vs/older-downloads/)
   * Openframeworks addon

## Cable Connections

**Broadcast**
* Plug each elgato capture card into a red (USB 3.0) port using a USB 3 extender cable (**important!**). Plug one in front and one in back of the PC
* Plug camera HDMIs into elgatos
* Plug camera USB power cables into a powered USB hub
* Plug camera fans into a powered USB hub (can be shared with cams)
* Connect audio source to Line-in port on rear of PC
* Connect PC to router with ethernet cable

**VR**
* Plug an **unpowered** USB hub (**ABSOLUTELY CRITICAL!**) into the red USB port under the PC's ethernet port on the back
* Plug headset USB cable into the hub
* Plug headset DisplayPort cable to port on back of PC
* Connect PC to router with ethernet cable
* Connect to monitor if using, otherwise plug in an HDMI dummy dongle to the port on the back (**important** make sure to plug the dummy into the HDMI port in the metal area of the PC. Left side if you are behind the PC)

## Startup

**Broadcast PC**
* Secure the cameras in the mount using tripod screws
* If not powered on yet, use the upper button presser arms on the mount to turn them on. Hold down for ~3 sec and you'll hear a long beep
* If you see a prompt to set the date and time, you can skip through it by pressing the large center button with red circle on the sides of the cameras repeatedly. This only comes up when the cameras have been disconnected from the USB power block for at least a few minutes.
* If you then get 3 quick beeps and the screen shows a camera symbol, they're ready and you can skip next step
* If the screen instead shows an HDMI logo, use the lower button presser arm to turn on live camera mode. This takes 3 presses and can't be done too quickly. After a short delay, you should hear 3 quick beeps and see the screen change to a camera symbol. Sometimes it needs an extra press or two.
* On the PC, find the TOTM_BROADCAST.exe shortcut on the desktop and start it

**VR**
If startup automation has been set up, the VR app will start automatically after starting the PC (after a minute or so).

Manual start:
* Make sure there is an Oculus software window open. If not, start it from the Start menu
* Find the TOTM_VR app build folder on the desktop and start the exe within

## Comms / Protocols

* **Media**
  * Audio and Video are streamed from Broadcast to VR PCs using RTP over the network broadcast address
* **Control**
  * Controls are sent from Broadcast to VR PCs using OSC, also via broadcast address
  * VR PCs listen to channel "/cmd" on port 8000
  * Possible string args:
    * `recenter` tells all VR apps to recenter the view when users are facing forward
    * `fadeout` and `fadein` trigger a fade to or from black within VR respectively
    * `blinkclosed` and `blinkopen` trigger an eye closing or opening effect within VR respectively
  * Additionally the VR PCs periodically send a `/heartbeat` message back to Broadcast on port 9000 to give their status


## Keyboard Controls

**Broadcast**
* `r` send OSC `/recenter` to VR PCs
* `f` alternately send OSC `/fadeout` and `/fadein` to VR PCs
* `b` alternately send OSC `/blinkopen` and `/blinkclosed` to VR PCs
* `c` swap cameras. Use this when the camera video streams are in the wrong L/R positions


**VR**
* `r` recenter headset
* `f` fadein / fadeout


## Development Install Guide

**Gstreamer**

Required for both apps

1. Go to the [download page](https://gstreamer.freedesktop.org/download/) and download both the runtime and development installers under **MinGW 64-bit** (MSVC VERSIONS WILL NOT WORK)
2. Install each one, making sure to select **COMPLETE** install when prompted. Leave the default install location as `C:\`
3. Add gstreamer binary folder path to System Environment Variables:
	1. Open Settings from the Start menu and type 'environment' in the search area
	2. Click the top result and then click  `Environment Variables...` on the bottom of the window that opens.
	3. Select `Path` in the list of variables and then click `Edit`
	4. In the new window, click `New` and then click 'Browse...`
	5. Go to `This PC` --> `Windows(C:)`-->`gstreamer`-->`1.0`-->`mingw_x86_64` and select `bin`, then click `OK`
	6. The new entry should read `C:\gstreamer\1.0\mingw_x86_64\bin`
	7. Click OK on the three windows you opened
	8. To see that the install was successful, open a command prompt and try `gst-launch-1.0 -h`
4. Add GSTREAMER_1_0_ROOT_X86_64 environment variable (required for Broadcast. The ofx addon looks for an environment variable with this name, which is different from the name set by the gstreamer installer)
	1. In the user environment variables there will be an entry for `GSTREAMER_1_0_ROOT_MINGW_x86_64`
	2. Make a new entry with the `MINGW` part removed: `GSTREAMER_1_0_ROOT_x86_64`
	3. Set the directory to the same one as for the exisiting variable: `C:\gstreamer\mingw_x86_64\`

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
7. choose 2019.4.27f1 (64-bit) [the latest LTR as of 07/09/2021] and go with defaults
8. Before you can use it, you need to log in with a free personal Unity account

**Oculus**

1. Download and install from https://www.oculus.com/setup/
2. install to default location
3. connect oculus to DisplayPort to port in rear
4. Connect an unpowered USB hub to one of the high-speed USB ports and plug the oculus into it
  * **IMPORTANT** Do not skip the hub part. Otherwise multiple unplug/replugs are often necessary to get the headset working
  * The exact USB port on the PC does not seem important, as long as it's red (USB 3.0)
6. Sign in to oculus
7. Start setup (requires one oculus controller)
8. Install firmware update(s) (replugging headset may be needed after an update)
9. Put headset on and follow instructions to set floor level and define ‘play area’


## Parameters / Configuration

Things we will likely want to alter or tweak in the future

* **Gstreamer broadcast pipeline**
  * The gstreamer pipeline is parameterized in [config.json](TOTM_BROADCAST/bin/data/config.json)
  * The main option string we may want to change is `video_encoding`.
    * More on video encoding options in [this doc](Docs/h264_encoding.md) (Note: we are using H265 now)
* **Camera Height**
  * Raising or lowering the virtual camera position relative to the center point of the spheres creates perception of growing and shrinking. Right now it it set below center to make viewer feel smaller
* **Fisheye K Parameter**
  * K angle parameter in [PixPro4kFishEye.shader](TOTM_VR/Assets/Resources/Shaders/PixPro4kFishEye.shader) controls FOV, so in effect this is a zoom parameter
  * The algorithm for projecting the fisheye camera frames onto the sphere geometry is implemented in the shader so this could potentially be changed. We had better results changing camera height for the desired amount of warping
    * More on fisheye projection in [this doc](Docs/fisheye_projection.md)
* **Periphery mask**
    * The width and height of the masked area can be adjusted within the Unity editor. [FisheyeKParam.cs](TOTM_VR/Assets/Scripts/FisheyeKParam.cs)
    * To change what the shader *does* with the masked area we would edit [PixPro4kFishEye.shader](TOTM_VR/Assets/Resources/Shaders/PixPro4kFishEye.shader)

## Unity Scripts & Shaders

#### Assets/Scripts
* FadeScript.cs (parameters: durations of fadein/fadeout. Attached to S-Left and S-Right objects)
* FisheyeKParam.cs (parameters: k angle, camera height, width and height of masked area. Attached to S-Left object and controls left and right eyes)
* OscScript.cs (OSC Command listener)
* VRRecenter.cs (sets S-Left and S-Right orientation to match headset forward. Triggered in OscScript.cs. Attached to S-Left and S-Right objects)
* HeadsetStatusScript.cs (Keeps track of headset sleep / awake status. Queried within other scripts)
* WaitingRoomImage.cs (Loads and controls movement and alpha of Dan Dare poster image)
* WaitingRoomScript.cs (Controls alpha levels of britta scene and dan dare image based on OSC commands)

#### Assets/GstreamerUnity/Components
* BaseVideoPlayer.cs (gstreamer pipeline listener / frame grabber. Writes to textures of SphereLeftMaterial and SphereRightMaterial)
* NetworkStereoPlayer.cs (sets gstreamer pipeline string)

#### Assets/Eye Blink Effect/Scripts
* BlinkEffect.cs (parameters: durations of eye open and eye close effects. Attached to LeftEyeAnchor and RightEyeAnchor components of OVRCameraRig)

#### Assets/Resources/Shaders
* PixPro4kFishEye.shader
  * pixel mapping from fisheye to spherical coords
  * Periphery mask is set here as an image file
  * FadeScript controls the "Multiplier" param for alpha fade.
  * Attached to SphereLeftMaterial and SphereRightMaterial, which are used by S-Left and S-Right

#### Assets/Eye Blink Effect/Shaders
* Blink Curved.shader (eye open / eye closed effects. Used by BlinkEffect component, change parameters in BlinkEffect.cs if needed)


## Possible Improvements
* Have TOTM_VR listen for parameters from BROADCAST. Have BROADCAST send params when a new client sends first heartbeat. Could add sliders in ofx app for some of these.
  * Network buffer time
  * Video dimensions (for scaling)
  * Fadein / fadeout time
  * Camera height (and maybe rotation)
  * Fisheye K value
  * Periphery mask width / height



## Misc Links
* [Big dev doc](https://docs.google.com/document/d/1dYk7BQNIYa2w9gIE8wetXxKf6VH5tuNBUVAaHY0cm_c) covering May - July 2021
* [Article](https://cloud.google.com/architecture/gpu-accelerated-streaming-using-webrtc#gstreamer_pipeline) on live streaming with gpu-accelerated gstreamer. I based parameters for the nvidia gpu encoder on the ones used here
* [ofx example](https://github.com/arturoc/ofxGstRTP) Example of using gstreamer within Openframeworks
* [gstreamer cheat sheet 1](https://github.com/matthew1000/gstreamer-cheat-sheet)
* [gstreamer cheat sheet 2](https://github.com/DamZiobro/gstreamerCheatsheet)
* [gstreamer tuning](https://developer.ridgerun.com/wiki/index.php/Embedded_GStreamer_Performance_Tuning)
* [gstreamer tutorials](https://gstreamer.freedesktop.org/documentation/tutorials/index.html?gi-language=c)
* [oculus debug tool](https://developer.oculus.com/documentation/native/pc/dg-debug-tool/?device=RIFT)

<br><br>

---

Document created by Patrick Rummage for [Brooklyn Research](https://brooklynresearch.com)

Last updated: September 22, 2022

