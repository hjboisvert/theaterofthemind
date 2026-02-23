# TOTM PCs Initial Setup

Get `TOTM.zip` from [Drive](https://drive.google.com/file/d/1zx1ros6qwOpIORFpEIfGmejEoZ7VTA0N/view?usp=sharing) and unzip on Desktop.
This folder contains the latest builds of TOTM_VR and TOTM_BROADCAST as well as all installers mentioned below.


## Contents
* [VR PCS](#vr_pcs)
* [Broadcast PC](#broadcast_pc)
* [All PCs](#all_pcs)

---

## VR PCs <a name="vr_pcs"></a>

### User
The Windows user is an offline, or local, account with a name following the pattern 'qwxx' where xx is a pair of digits in the range 01 - 18.
The password is Pizzaqwxx, again replacing xx with the number of the user.

### NVIDIA Driver
If PC has an Nvidia driver other than `457.51`, use the installer in `TOTM\installers`.
Choose the option for Driver only, **without GeForce Experience**.

### TightVNC
Download and install the Windows 64-bit version from [here](https://tightvnc.com/download.php).
Typical installation with default options is fine.
For the password, use the same as for the Windows user

### Oculus Client
Scroll down to Rift S on [this page](https://www.oculus.com/setup/) and download and run the installer

You will need to log in to Oculus.
User: totm.bkr@gmail.com
Password: OTB4ccount

It will also prompt for a security code.
Select the second option, which allows using codes from a list.

A few codes:
39710024
44258745
46269129
61426730

Note: the installer will download >5 GB additional data

#### **Version Rollback**
Because there is no official way to install a specific oculus version, we need to manually roll back the oculus files to match the
version on the other PCs.

Go to C:\Program Files\Oculus\Support
Look at the folders included in `install\oculus_v39_0_0_65_369.zip` and delete ones with those names in the Support folder.
Unzip the files and copy into Support folder.

**NOTE**: The client app will still unfortunately still show whatever version was initially installed. This is expected.
You can check the version of these support files by launching Support\oculus-diagnostics\OculusDebugTool.exe and checking system info.

### gstreamer

Install gstreamer version 1.18.6. Link, if needed: [Link](https://gstreamer.freedesktop.org/data/pkg/windows/1.18.6/mingw/)
There are 2 installers:
gstreamer-1.0-mingw-x86_64-1.18.6.msi
gstreamer-1.0-devel-mingw-x86_64-1.18.6.msi

Install both, making sure to select **COMPLETE** install when prompted.

Add gstreamer binary folder path to System Environment Variables:
1. Open Settings from the Start menu and type 'environment' in the search area
2. Click the top result and then click  “Environment Variables…” on the bottom of the window that opens.
3. Select “Path” in the list of variables and then click “Edit”
4. In the new window, click “New” and then click “Browse…”
5. Go to “This PC” --> “Windows(C:)”-->”gstreamer”-->”1.0”-->”mingw_x86_64” and select “bin”, then click “OK”
6. The new entry should read “C:\gstreamer\1.0\mingw_x86_64\bin”
7. Click OK on the three windows you opened
8. To see that the install was successful, open a command prompt and try “gst-launch-1.0 –version”


### Node.js
Install Node.js version 16.14.x.
This is the current LTS release as of 03/21/2022.
Don't check the box to install extra build tools, they're not necessary.

### PM2
Once node is installed, open a cmd window and run `npm install -g pm2` to install pm2

### Python
Run the Python-3.10.6-amd64.exe installer and make sure to **check box for "Add python 3.10 to PATH"** keep defaults for all other
options.

Note where it installs to. Typically it will be `C:\Python310`.
Delete the `site-packages` folder in `Python310\Lib` and replace it with the one in the install folder of TOTM.zip, unzipping first.


## TOTM Folder

## Startup Automation

### Disable Password
Open the Start menu (windows icon on lower left) and right click the user portrait and then "change account settings".
In the window that opens, click "Sign-in options" and under "Require sign-in", select "Never".

#### AutoLogon
On new installs of windows, the above steps will not work. Intead, run AutoLogon.exe in TOTM\install\AutoLogon and enter
the username and password in the dialog. See User section above for the user and password to use here.

### Place Shortcuts in Startup Folder
#### Open Startup Folder
Press WIN+r to open the "run" prompt and type `shell:startup` in the text box, then click `Run`. This will open the Startup directory. Any shortcuts to executables placed here will be run once the current user logs in.

#### Oculus Client
Open Start menu and find Oculus. Right click and then over over "More" and click "Open file location". In the window that opens, copy the "Oculus" shortcut and paste it into the startup folder opened earlier.

#### Startup Script
In the "TOTM\VR" folder, right click "STARTUP.BAT" and choose "Create Shortcut". Copy the shortcut and paste in startup folder.

**NOTE**: Open pm2.config.js in Notepad and confirm that the folder name of the unity app build is correct.
Usually, the folder name will be the date of the build (for example: 2_28_2022), so this file may need to be updated for new builds.

**NOTE**: TOTM_VR.exe needs to be launched manually the first time it runs. A windows defender prompt will appear and you need to click “Allow”.

**NOTE**: If you see a console error like "ImportError: No module named ait" after the startup script launches,
    that means the `site-packages` folder hasn't been replaced. See Python section above.

---


## BROADCAST PC<a name="broadcast_pc"></a>

Username for this PC is not important.
Disable password log-in following instructions under “Startup Automation” above.

## TightVNC
Download and install the Windows 64-bit version from [here](https://tightvnc.com/download.php)
Typical installation with default options is fine.
For the password, use the same as for the Windows user

## gstreamer
Follow gstreamer instructions from the VR PC section above, then

### Update environment variables
1. Start typing “environment” in the “Type here to search” area of the taskbar
2. Click “Edit the System Environment Variables” when it appears.
3. Add GSTREAMER_1_0_ROOT_X86_64 environment variable
	1. In the user environment variables there will be an entry for `GSTREAMER_1_0_ROOT_MINGW_x86_64`
	2. Make a new entry with the `MINGW` part removed: `GSTREAMER_1_0_ROOT_x86_64`
	3. Set the directory for this variable to the same one as for the exisiting variable: `C:\gstreamer\mingw_x86_64\`


## Elgato
Install 4k Capture Utility in the installers folder.
Installing the utility will also install the required drivers for the capture cards.


## TOTM_BROADCAST
In the `TOTM\BROADCAST\bin` folder, right click on TOTM_BROADCAST.exe then “Create Shortcut”
Place the new shortcut on the desktop

---

## All PCs <a name="all_pcs"></a>

* Uninstall Norton Security
  * Type "Add" in the text area on the desktop taskbar, click "Add or Remove Programs" in the menu that opens
  * Find Norton in the list and click "uninstall"
* Turn off wifi
* Once the PC is on the local network, make sure that the network is designated as 'private' (trusted).
  * Click on the network icon in the lower right
  * Under the "Unidentified" network, click "Properties"
  * Under "Network Profile", make sure "Private" is selected
* Follow instructions in the [disable notifications and updates guide](./win_notifications_updates.md)

<br><br>

---

Document created by Patrick Rummage for [Brooklyn Research](https://brooklynresearch.com)

Last updated: September 22, 2022

