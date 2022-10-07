from os import getlogin, listdir
from os.path import isfile, join, getctime
import time
import ctypes
import ait
import subprocess

print("\n")
print("BABYSITTER LAUNCH")
print("Waiting 60 sec to catch boot stall")
time.sleep(60)

MAX_REBOOTS = 5

username = getlogin()
home_path = "C:\\Users\\" + username

boot_stall_str = "{!ERROR!} [ThreadLoop] STALL DETECTED!!"

reboot_file_path = home_path + "\\Documents\\reboot_count.txt"

REBOOTING = False

def reset_reboot_count():
    with open(reboot_file_path, 'w+') as f: # creates file if it doesnt exist
        f.seek(0)
        f.write('0')

def trigger_reboot():
    with open(reboot_file_path, 'w+') as f: # creates file if it doesnt exist
        f.seek(0)
        line = f.readline()
        if len(line) > 0:
            if (int(line) >= MAX_REBOOTS):
                return
            else:
                f.write(str(int(line) + 1)) # increment reboot count
        else:
            f.write('1')


    print("REBOOTING")
    time.sleep(1) # Let it finish writing and close file
    subprocess.call("shutdown /r /t 15", shell=True)
    time.sleep(1)
    quit()

def watch(file_path):
    rebooting = False
    fp = open(file_path, 'r')
    print("Checking for boot stall...")
    first_pass = True
    while first_pass: # read through messages logged before script launch
        line = fp.readline()
        if len(line) == 0: #EOF
            first_pass = False
            print("No boot stall")
            reset_reboot_count()
        elif boot_stall_str in line:
            first_pass = False
            print("FOUND BOOT STALL!")
            rebooting = True
            trigger_reboot()

    if rebooting:
        return ''

    print("Watching log file...")
    while True: # Check for new line every 0.5 sec
        new_line = fp.readline()

        if new_line:
            yield new_line
        else:
            time.sleep(0.5)



file_path = "C:\\Users\\" + username + "\\AppData\\Local\\Oculus\\"

file_list = [f for f in listdir(file_path) if isfile(join(file_path,f)) and f[:8] == "Service_"]

print("found files: ")
print(file_list)

latest = max(file_list, key=lambda x: getctime(join(file_path,x)))

print("")
print("Latest = %r " % latest)

setup_str = "INSIDE_OUT_GUARDIAN_SETUP" # this matches on play area trigger AND when you click skip
play_area_str = "[SetupManager] New NUX Step: INSIDE_OUT_GUARDIAN_SETUP" # this matches ONLY on play area trigger


def find_oculus_window():
    return ctypes.windll.user32.FindWindowW(0, "Oculus")


def get_window_rect(hwnd):
    rect = ctypes.wintypes.RECT()
    ctypes.windll.user32.GetWindowRect(hwnd, ctypes.pointer(rect))
    return (rect.left, rect.top, rect.right, rect.bottom)

def skip_guardian():
    time.sleep(2)
    hwnd = find_oculus_window()

    window_rect = get_window_rect(hwnd)
    w = window_rect[2] - window_rect[0]
    h = window_rect[3] - window_rect[1]
    print("FOCUS WINDOW")
    ctypes.windll.user32.SwitchToThisWindow(hwnd, False)
    skip_btn_x = window_rect[0] + w * 0.416
    skip_btn_y = window_rect[1] + h * 0.7083
    ait.move(skip_btn_x, skip_btn_y)
    ait.click()
    time.sleep(2)
    subprocess.call("taskkill /f /im TOTM_VR.exe", shell=True)


for line in watch(join(file_path, latest)):
    if play_area_str in line:
        print("LOG LINE: % r" % (line))
        skip_guardian()

    elif setup_str in line:
        m = ait.mouse()
        print("MOUSE: % r, % r" % (m[0], m[1])) # this prints skip button x, y when its clicked



