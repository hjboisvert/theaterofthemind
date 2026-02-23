# TOTM PCs Initial Setup

TODO: update gdrive link
Get `TOTM.zip` from [Drive](https://drive.google.com/file/d/1zx1ros6qwOpIORFpEIfGmejEoZ7VTA0N/view?usp=sharing) and unzip on Desktop.
This folder contains the latest builds of TOTM_VR and TOTM_BROADCAST as well as all installers referenced below.


## Contents
* [Asus PCs](#asus_pcs)
* [VR PCS](#vr_pcs)
* [Broadcast PC](#broadcast_pc)
* [Updates to MSI Trident PCs](#trident_updates)

---

### Asus PCs <a name="asus_pcs"></a>

#### Windows 11

##### User
During install, create an offline local user account
* Press shift + F10 at the Microsoft sign-in screen to open a console window 
* run `start ms-cxh:localonly`

Then in the Local Account setup screen
* User name: `AsusVRxx` (replace xx with the pc number)
* Password: `TOTM`

Security Questions
* Pet's Name: `TOTM`
* Name of city: `TOTM`
* Childhood nickname: `TOTM`

##### Disable USB Power Saving
* Open Device Manager and expand USB devices
* For each item in the list:
  * open the settings menu and look for the Power Management tab
    * Note: not every USB device has this tab, ignore those
  * Uncheck the option “Allow the computer to turn off this device..”
* The “root router” device has the option “Allow this device to wake the computer” checked. Do not change this setting

##### Disable PC Sleep
* Type power settings in the desktop search bar and open Power and Sleep settings 
* Set the PC to never sleep and to shut off when the power button is pressed


#### NVIDIA Driver
Use the installer in `TOTM\installers` to install driver version `591.74`.
Choose the option for Driver only, **without GeForce Experience**.

#### TightVNC
Install TightVNC version `2.8.85` from the installer folder or from [here](https://tightvnc.com/download.php).
Typical installation with default options is fine.
For the password, use `TOTM`

#### Meta Horizon Link Desktop App
Run the `OculusSetup.exe` installer. Requires internet during install and login

Then log in to Oculus

User: totmarbutus@gmail.com

Click Settings on the lower left, then in the "General" tab, click the option on the bottom to Set Meta Horizon Link as the active OpenXR Runtime


#### gstreamer

Install gstreamer version 1.18.6. Link, if needed: [Link](https://gstreamer.freedesktop.org/data/pkg/windows/1.18.6/mingw/)
There are 2 installers:
gstreamer-1.0-mingw-x86_64-1.18.6.msi
gstreamer-1.0-devel-mingw-x86_64-1.18.6.msi

Install both, making sure to select **COMPLETE** install when prompted.

##### Update system path
Add gstreamer binary folder path to System Environment Variables:
1. Open Settings from the Start menu and type 'environment' in the search area
2. Click the top result and then click  “Environment Variables…” on the bottom of the window that opens.
3. Select “Path” in the list of variables and then click “Edit”
4. In the new window, click “New” and then click “Browse…”
5. Go to “This PC” --> “Windows(C:)”-->”gstreamer”-->”1.0”-->”mingw_x86_64” and select “bin”, then click “OK”
6. The new entry should read “C:\gstreamer\1.0\mingw_x86_64\bin”
7. Click OK on the three windows you opened
8. To see that the install was successful, open a command prompt and try “gst-launch-1.0 –version”

##### Update environment variables
1. Start typing “environment” in the “Type here to search” area of the taskbar
2. Click “Edit the System Environment Variables” when it appears.
3. Add GSTREAMER_1_0_ROOT_X86_64 environment variable
	1. In the user environment variables there will be an entry for `GSTREAMER_1_0_ROOT_MINGW_x86_64`
	2. Make a new entry with the `MINGW` part removed: `GSTREAMER_1_0_ROOT_x86_64`
	3. Set the directory for this variable to the same one as for the exisiting variable: `C:\gstreamer\mingw_x86_64\`

#### Node.js
Use the Node.js installer to install version 24.13.0.
Don't check the box to install extra build tools, they're not necessary.

#### PM2
Once node is installed, open a cmd window and run `npm install -g pm2` to install pm2

#### AutoLogon
Run AutoLogon.exe in TOTM\install\AutoLogon and enter
the username and password in the dialog. See User section above for the user and password to use here.

#### Set LAN to Private
Once the PC is on the local network, make sure that the network is designated as 'private' (trusted)
  * Click on the network icon in the lower right
  * Under the "Unidentified" network, click "Properties"
  * Under "Network Profile", make sure "Private" is selected

## VR PCs <a name="vr_pcs"></a>
### Place Shortcuts in Startup Folder
#### Open Startup Folder
Press WIN+r to open the "run" prompt and type `shell:startup` in the text box, then click `Run`. This will open the Startup directory. Any shortcuts to executables placed here will be run once the current user logs in.

#### Meta Horizon Link Desktop App
Open Start menu and find Meta Horizon Link. Right click and then over over "More" and click "Open file location". In the window that opens, copy the `Meta Horizon Link` shortcut and paste it into the startup folder opened earlier.

#### Startup Script
In the "TOTM\VR" folder, right click "STARTUP_v2.BAT" and choose "Create Shortcut". Copy the shortcut and paste in startup folder.

**NOTE**: Open pm2.config.js in Notepad and confirm that the folder name of the unity app build is correct.
Usually, the folder name will be the date of the build (for example: 2_28_2022), so this file may need to be updated for new builds.

**NOTE**: TOTM_VR.exe needs to be launched manually the first time it runs. A windows defender prompt will appear and you need to click “Allow”.

---


## BROADCAST PC<a name="broadcast_pc"></a>

### TOTM_BROADCAST
In the `TOTM\BROADCAST\bin` folder, right click on TOTM_BROADCAST.exe then “Create Shortcut”
Place the new shortcut on the desktop

---



## Updates to MSI Trident PCs <a name="trident_updates"></a>
The tridents PCs from the 2022 run of the show are the backup VR PCs and have been updated to work with the Quest Headset

* Many of the coin cell batteries on the motherboards were dead, which caused the PC to wait for keyboard input on BIOS
startup. Batteries have been replaced in all PCs
* Windows updated to the final Windows 10 patch
* Nvidia driver updated to version `581.57`
* Oculus client uninstalled, replaced by Meta Horizon Link app