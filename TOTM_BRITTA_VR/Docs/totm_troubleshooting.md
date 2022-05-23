# TOTM TROUBLESHOOTING

First, check the Cable Connections and Startup sections of the readme

## Contents
* [VR PCs](#vr_pcs)
* [Broadcast PC](#broadcast_pc)

<br>

---

## VR PCs <a name="vr_pcs"></a>

* Need to exit TOTM_VR manually, but it keeps reopening.

    * **Cause**: It is being launched by a daemon process (pm2) as part of the startup automation

    * **Solution**: Open a CMD window (there should already be one open from pm2 launching, but if it was closed you can start a new one) and run command “pm2 stop 0”

<br>

* Getting video stream in the headset but there’s no audio.

    * **Cause**: wrong audio output device selected on PC

    * **Solution**: on the PC desktop click on the sound icon on the lower
right and make sure the selected device says
“Rift Headphones” and not “Rift Virtual Audio”.
This selection persists after reboot so should not need
to be repeated.

<br>

* TOTM_VR starts up but in the headset there is gray where the video stream should be. You can look around and see the black masked area to the sides.

    * **Cause**: This means that the PC is not receiving the video stream  over the network.

    * **Solution**: Verify that TOTM_BROADCAST is running on the broadcast PC and that it is not displaying an error message.

        If broadcast PC looks OK, check that the VR PC is on the right network and
		* WIFI is turned off
		* The wired LAN connection is using the “private”, or trusted, profile
        * Check that gstreamer bin folder is in the system PATH env variable (see [PC setup guide](./totm_pc_setup.md))

        If none of the above fixes it, close TOTM_VR and launch it         manually to see if a Windows Defender window appears.

        If yes, click “Allow”. This will only happen for the first run of the app on a machine. However if we make changes to the totm_vr app itself, it will be necessary to “allow” once again (because the executable will be different).

<br>

* TOTM_VR does not start automatically. There is a CMD window open with an error message saying “pm2.config.js not found”.

    * **Cause**: The location of the pm2.config.js file does not match the one given in STARTUP.BAT

    * **Solution**: The path to the pm2.config.js file is set to “C:\Users\%USERNAME%\Desktop\TOTM\VR\pm2.config.js”. In at least one of the PCs, the Desktop folder is NOT located under a folder with the name of the user (it’s probably an older username that was first used to set up the PC before they were numbered). Update that line in STARTUP.BAT to match the real path to the pm2.config.js file.

<br>

* Similar to above but the error message in the CMD window says “ ‘pm2’ is not recognized as an internal or external command, operable program or batch file.”

    * **Cause**: pm2 is either not installed or was installed incorrectly. Most likely the “-g” flag was left out when running the install command.

    * **Solution:** Open a cmd window and run “npm install -g pm2”

<br>

* TOTM_VR is stuck in a loop where it closes a few seconds after opening and then reopens

    * **Cause**: Windows Security is blocking the app because it is launched by a script. Note: I only saw this happen on a PC that had been wiped and had a fresh windows install.

    * **Solution**: Open windows security center from the task bar or main settings window, go to App and Browser Control and uncheck all options under Reputation-based protection.
    If that doesn't work, check `pm2.config.js` and make sure it is launching the latest version of TOTM_VR

<br>

* Oculus client shows green headset status, but then after restarting the PC,  the headset status shows a red circle with an X. Sometimes, but not always, accompanied by noticeable issue with the headset, like poor motion tracking when you try looking around. Unplugging the headset from USB, then plugging in again gives green status but only until the next PC power cycle.

    * **Cause**: Headset cable USB

    * **Solution**: The only permanent fix is to swap the cable. Otherwise you can unplug the USB from the hub, wait a few seconds, then plug in again. That worked 100% of the time for the one cable that had this problem.

<br>

---

## Broadcast PC<a name="broadcast_pc"></a>

* TOTM_BROADCAST window shows a red error message like “[ERROR] PIPELINE FAILED: Error from element wasapisrc: Could not open resource for reading.”

    * **Cause**: Could not start the audio stream

    * **Solution**: Make sure the audio source is turned on and plugged in to the Line-in port on the PC

<br>

* Like above but the message has “element right” or “element left” in place of “element wasapisrc”

    * **Cause**: Could not start video stream for one or both cameras (it displays the first critical error, so both sides may be failing even though it only says right or left in the message)

    * **Solution**: Make sure both cameras are connected properly and turned on

<br>

* TOTM_BROADCAST window shows no errors but one or both camera streams just shows large text “HDMI”

    * **Cause**: Camera is in the wrong mode.

    * **Solution**: Check the LCD displays on the tops of the cameras. If one or both is displaying an HDMI logo, use the lower button press arm on the mount to press the lower button 3 times (doesn’t work if you do it too fast, sometimes need to press a couple extra times). The display will show a video camera symbol and flash the text “NO CARD” and then switch to streaming mode

<br>

* Camera suddenly beeps loudly and shuts off

    * **Cause**: The camera overheated

    * **Solution**: Make sure that the fans are on. The cameras should never overheat with the fans running

<br><br>

---

Document created by Patrick Rummage for [Brooklyn Research](https://brooklynresearch.com)

Last updated: May 17, 2022


