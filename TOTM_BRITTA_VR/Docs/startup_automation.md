# VR PC Startup Automation

We are using [PM2](https://pm2.keymetrics.io/) to launch the Unity app

**Important text files**
Startup script: Desktop\TOTM\VR\STARTUP.BAT
pm2 config file: Desktop\TOTM\VR\pm2.config.js

**startup folder**
Shortcuts to both the oculus client and the startup script are placed in the 'startup' folder. Access this folder by pressing WIN+r on the keyboard and typing 'shell:startup' in the text box that appears, then click 'run'.

**Startup sequence:**
1. Password auth is diabled, so user qw\<xx\> logs in automatically (where xx is one of 01 - 18)
2. Oculus software starts
3. STARTUP.BAT starts
	1. 45 second delay to allow oculus software to find & initialize heasdset
	2. Script launches britta app indirectly by launching pm2 with config file pm2.config.js. This config file has the path to the Britta app exe.

To stop the Britta app and not have pm2 re-launch it:
run `pm2 stop 0` in any cmd window

**Note**: Both the startup script and pm2 config file assume that the path to the TOTM folder on the desktop follows the pattern "C:\Users\<USERNAME>\Desktop\TOTM". If this is not the case on a particular PC, the paths in those text files will need to be updated.
