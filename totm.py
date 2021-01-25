# This script is intended to send OSC messages on button presses and releases
# Comments in individual sessions to follow.
# In-line comments are where to change IP address and Port

# Importing time management functions.
from time import sleep
from signal import pause

# Sleeping for 5 seconds to allow for modules to load before script runs.
sleep(5)

# Importing GPIO Library, specifically the LED and Button functions.
# LED function to be attempted as relay control.
from gpiozero import LED, Button

# Importing OSC functions. Currently not using the Server functionality in this script.
from pythonosc import osc_message_builder
from pythonosc import udp_client
from pythonosc import dispatcher
from pythonosc import osc_server

# Importing other systems. Some of these could be removed.
import argparse
import random
import sys
import socket

# Declaring variables. The number here references the BCM number. Run "gpio readall" from CLI.
hostname=socket.gethostname()
relay1=LED(21)
relay2=LED(20)
relay3=LED(16)
relay4=LED(12)
buttonA=Button(2)
buttonB=Button(3)
buttonC=Button(4)
buttonD=Button(17)
buttonE=Button(27)
buttonF=Button(22)
buttonG=Button(10)
buttonH=Button(9)
buttonI=Button(11)
buttonJ=Button(0)
buttonK=Button(5)
buttonL=Button(6)
buttonM=Button(13)
buttonN=Button(19)
buttonO=Button(26)

# OSC Messages to be sent. See documentation for key.
# Format /hostname/identifier variable
def msgA1():
    client.send_message("/" + hostname + "/A", 1)
    print("Sent " + hostname + " A1.")

def msgA0():
    client.send_message("/" + hostname + "/A", 0)
    print("Sent " + hostname + " A0.")

def msgB1():
    client.send_message("/" + hostname + "/B", 1)
    print("Sent " + hostname + " B1.")

def msgB0():
    client.send_message("/" + hostname + "/B", 0)
    print("Sent " + hostname + " B0.")

def msgC1():
    client.send_message("/" + hostname + "/C", 1)
    print("Sent " + hostname + " C1.")

def msgC0():
    client.send_message("/" + hostname + "/C", 0)
    print("Sent " + hostname + " C0.")

def msgD1():
    client.send_message("/" + hostname + "/D", 1)
    print("Sent " + hostname + " D1.")

def msgD0():
    client.send_message("/" + hostname + "/D", 0)
    print("Sent " + hostname + " D0.")

def msgE1():
    client.send_message("/" + hostname + "/E", 1)
    print("Sent " + hostname + " E1.")

def msgE0():
    client.send_message("/" + hostname + "/E", 0)
    print("Sent " + hostname + " E0.")

def msgF1():
    client.send_message("/" + hostname + "/F", 1)
    print("Sent " + hostname + " F1.")

def msgF0():
    client.send_message("/" + hostname + "/F", 0)
    print("Sent " + hostname + " F0.")

def msgG1():
    client.send_message("/" + hostname + "/G", 1)
    print("Sent " + hostname + " G1.")

def msgG0():
    client.send_message("/" + hostname + "/G", 0)
    print("Sent " + hostname + " G0.")

def msgH1():
    client.send_message("/" + hostname + "/H", 1)
    print("Sent " + hostname + " H1.")

def msgH0():
    client.send_message("/" + hostname + "/H", 0)
    print("Sent " + hostname + " H0.")

def msgI1():
    client.send_message("/" + hostname + "/I", 1)
    print("Sent " + hostname + " I1.")

def msgI0():
    client.send_message("/" + hostname + "/I", 0)
    print("Sent " + hostname + " I0.")

def msgJ1():
    client.send_message("/" + hostname + "/J", 1)
    print("Sent " + hostname + " J1.")

def msgJ0():
    client.send_message("/" + hostname + "/J", 0)
    print("Sent " + hostname + " J0.")

def msgK1():
    client.send_message("/" + hostname + "/K", 1)
    print("Sent " + hostname + " K1.")

def msgK0():
    client.send_message("/" + hostname + "/K", 0)
    print("Sent " + hostname + " K0.")

def msgL1():
    client.send_message("/" + hostname + "/L", 1)
    print("Sent " + hostname + " L1.")

def msgL0():
    client.send_message("/" + hostname + "/L", 0)
    print("Sent " + hostname + " L0.")

def msgM1():
    client.send_message("/" + hostname + "/M", 1)
    print("Sent " + hostname + " M1.")

def msgM0():
    client.send_message("/" + hostname + "/M", 0)
    print("Sent " + hostname + " M0.")

def msgN1():
    client.send_message("/" + hostname + "/N", 1)
    print("Sent " + hostname + " N1.")

def msgN0():
    client.send_message("/" + hostname + "/N", 0)
    print("Sent " + hostname + " N0.")

def msgO1():
    client.send_message("/" + hostname + "/O", 1)
    print("Sent " + hostname + " O1.")

def msgO0():
    client.send_message("/" + hostname + "/O", 0)
    print("Sent " + hostname + " O1.")

# Lining up a button with a message. See documentation for key.
buttonA.when_pressed = msgA1
buttonA.when_released = msgA0
buttonB.when_pressed = msgB1
buttonB.when_released = msgB0
buttonC.when_pressed = msgC1
buttonC.when_released = msgC0
buttonD.when_pressed = msgD1
buttonD.when_released = msgD0
buttonE.when_pressed = msgE1
buttonE.when_released = msgE0
buttonF.when_pressed = msgF1
buttonF.when_released = msgF0
buttonG.when_pressed = msgG1
buttonG.when_released = msgG0
buttonH.when_pressed = msgH1
buttonH.when_released = msgH0
buttonI.when_pressed = msgI1
buttonI.when_released = msgI0
buttonJ.when_pressed = msgJ1
buttonJ.when_released = msgJ0
buttonK.when_pressed = msgK1
buttonK.when_released = msgK0
buttonL.when_pressed = msgL1
buttonL.when_released = msgL0
buttonM.when_pressed = msgM1
buttonM.when_released = msgM0
buttonN.when_pressed = msgN1
buttonN.when_released = msgN0
buttonO.when_pressed = msgO1
buttonO.when_released = msgO0

# This is what to do upon receiving a message.
# Currently activated when receiving /relayx/control y
# Where x is which relay 1-4 and y is 1 or 0.
# Print line can be removed if problematic.
def handle_relay1(unused_addr,args, r1):
    print("Received relay1",r1)
    if r1==1:
        relay1.on()
    else:
        relay1.off()

def handle_relay2(unused_addr,args, r2):
    print("Received relay2",r2)
    if r2==1:
        relay2.on()
    else:
        relay2.off()

def handle_relay3(unused_addr,args, r3):
    print("Received relay3",r3)
    if r3==1:
        relay3.on()
    else:
        relay3.off()

def handle_relay4(unused_addr,args, r4):
    print("Received relay4",r4)
    if r4==1:
        relay4.on()
    else:
        relay4.off()

# Main body of script, IP addresses and ports can be changed where noted inline.
if __name__ == "__main__":
    try:
        # Defining arguments. rip for Receiving IP, rport for Receiving Port,
        # sip for Sending IP, sport for Sending Port
        parser = argparse.ArgumentParser()
        parser.add_argument("--rip", default="192.168.1.17", help="The IP to listen to") #Change local IP here
        parser.add_argument("--rport", type=int, default="53003", help="The port to listen on") #Change receive port here
        parser.add_argument("--sip", default="192.168.1.10", help="The IP to send to") #Change remote IP here
        parser.add_argument("--sport", type=int, default="53002", help="The Port to send on") #Change sending port here
        args = parser.parse_args()
        # Defining variables for further use
        rip=args.rip
        rport=args.rport
        # Start OSC client
        client=udp_client.SimpleUDPClient(args.sip, args.sport)
        # Define OSC strings to listen for.
        dispatcher = dispatcher.Dispatcher()
        dispatcher.map("/relay1/control",handle_relay1,"r1") #Change OSC Strings Here
        dispatcher.map("/relay2/control",handle_relay2,"r2") #Change OSC Strings Here
        dispatcher.map("/relay3/control",handle_relay3,"r3") #Change OSC Strings Here
        dispatcher.map("/relay4/control",handle_relay4,"r4") #Change OSC Strings Here
        dispatcher.map("/testprint",print) #Change OSC Strings Here
        # Start OSC Server
        server = osc_server.ThreadingOSCUDPServer((args.rip, args.rport), dispatcher)
        # Print some startup information.
        print("Serving on {}".format(server.server_address))
        print("Sending from: " + hostname)
        print("Ready.")
        # Run the server until it is stopped.
        server.serve_forever()
    except KeyboardInterrupt:
        print("\nServer stopped")
    except OSError as err:
        print("OSC server error",err.args)

# Written by Gregory W. Towle