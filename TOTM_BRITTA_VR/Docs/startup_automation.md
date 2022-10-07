# VR PC Startup Automation

We are using [PM2](https://pm2.keymetrics.io/) to launch the Unity app

**Important text files**
Startup script: Desktop\TOTM\VR\STARTUP.BAT
pm2 config file: Desktop\TOTM\VR\pm2.config.js
python script: Desktop\TOTM\VR\totm_python\oculus_babysitter.py

**startup folder**
Shortcuts to both the oculus client and the startup script are placed in the 'startup' folder. Access this folder by pressing WIN+r on the keyboard and typing 'shell:startup' in the text box that appears, then click 'run'.
If you are using remote desktop and can't use the WIN key, you can also open the Run dialog via the Task Manager with File -> Run New Task

**Startup sequence:**
1. Password auth is diabled, so user qw\<xx\> logs in automatically (where xx is one of 01 - 18)
2. Oculus software starts
3. STARTUP.BAT starts
	1. 45 second delay to allow oculus software to find & initialize heasdset
	2. Script launches britta app indirectly by launching pm2 with config file pm2.config.js. This config file has the path to the Britta app exe.
    3. pm2 launches python script in VR\totm_python that watches for desktop play area prompt and clicks continue button.

To stop the Britta app and not have pm2 re-launch it:
run `pm2 stop 0` in any cmd window

**Note**: Both the startup script and pm2 config file assume that the path to the TOTM folder on the desktop follows the pattern "C:\Users\<USERNAME>\Desktop\TOTM". If this is not the case on a particular PC, the paths in those text files will need to be updated.

**Python Script:**
Solves two issues:

* Occasional play area or guardian setup sequence that still sometimes occurs on headset wakeup. The script watches the oculus log
file for a key line and then clicks through the dialog that pops up, returning VR focus to the TOTM_VR app.

* Oculus client freeze. This sometimes happens when the PC boots and the only fix is a computer restart. The script watches oculus logs
and triggers a reboot when a freeze is detected. **NOTE**: it will only do this a maximum of 5 times in a row if oculus keeps freezing,
to prevent an endless reboot cycle. The counter gets reset on successful oculus launch.

<br><br>

---

Document created by Patrick Rummage for [Brooklyn Research](https://brooklynresearch.com)

Last updated: September 22, 2022


