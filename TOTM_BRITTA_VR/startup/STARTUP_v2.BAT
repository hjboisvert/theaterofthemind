netsh advfirewall set privateprofile state off

:WAIT_FOR_LINK
powershell -Command "Get-Process OculusDash -ErrorAction SilentlyContinue" | findstr /I "OculusDash" >nul 2>&1
if %errorlevel% neq 0 (
    timeout /T 5 /nobreak >nul
    goto WAIT_FOR_LINK
)

echo Link is ready! Starting BRITTA after warmup...
timeout /T 15

CMD /c "start pm2 start %HOMEPATH%\Desktop\TOTM\VR\pm2.config.js"