timeout /T 45
netsh advfirewall set privateprofile state off
CMD /c "start pm2 start %HOMEPATH%\Desktop\TOTM\VR\pm2.config.js"