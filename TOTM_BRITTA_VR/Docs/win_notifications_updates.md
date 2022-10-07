# Disable Notifications and Automatic Updates on Windows

## Notifications

  * Start Menu
  * Settings
  * Start typing "notifications" in search box
  * Select "Notifications & actions settings"
  * Switch main toggle to off
  * Uncheck all boxes below

## Focus Assist

[Link](https://support.microsoft.com/en-us/windows/focus-assist-automatic-activation-settings-81ed1b25-809b-741d-549c-7696474d15d3)

* Search for Focus Assist in the system settings window
* Select "Alarms only" in the top part and turn off all automatic rules below


## Disable Automatic Windows Updates

The PCs should be permanently offline. This is a fallback defense in case a PC is put on a network with internet by mistake.

In Windows Home Edition, we need to add an entry to the Windows Registry. In Pro it can be done with Group Policy.
[Source](https://www.windowscentral.com/how-stop-updates-installing-automatically-windows-10)


  * WINDOWS_KEY + R then type "regedit" and click Run
    * If you are using remote desktop or otherwise cannot use the windows key, you can open task manager and then click File -> Run New Task to open the run prompt

  * Navigate to the following path: `HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows`

  * If the folders `WindowsUpdate` and `WindowsUpdate\AU` already exist, skip the next 4 steps

  * Right-click the **Windows** (folder) key, select the **New** submenu, and then choose the **Key** option

  * Name the new key **WindowsUpdate** and press Enter

  * Right-click the new **WindowsUpdate** key, select the **New** submenu, and choose the **Key** option.

  * Name the new key **AU** and press Enter

  * Right-click the new **AU** key, select the **New** submenu, and choose the **DWORD (32-bit) Value** option

  * Name the new key NoAutoUpdate and press Enter

  * Double-click the newly created key and change its value from 0 to **1**

  * Click the OK button

  * Restart computer

<br><br>

---

Document created by Patrick Rummage for [Brooklyn Research](https://brooklynresearch.com)

Last updated: September 22, 2022


