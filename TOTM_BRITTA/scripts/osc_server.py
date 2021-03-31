"""Theater of the Mind :: Britta Multisensory Controller :: OSC server

This program listens to instructions from Medialon on several OSC addresses

Gershon Dublon
slow immediate LLC
March 2020
"""
import argparse
import RPi.GPIO as GPIO
from time import sleep
from pythonosc import dispatcher
from pythonosc import osc_server
import socket

def run_diffuser(unused_addr, args, duration):
    print("run diffuser for {0}s".format(duration), flush=True)
    for duty in range(0,101,1):
        args[0].ChangeDutyCycle(duty) #provide duty cycle in the range 0-100
        sleep(0.0125)
    sleep(duration)
    args[0].ChangeDutyCycle(0)

def run_fan(unused_addr, args, duration, wind_intensity=100):
    print("run fan at {1} for {0}s".format(duration, wind_intensity), flush=True)
    args[0].ChangeDutyCycle(wind_intensity)   #provide duty cycle in the range 0-100
    sleep(duration)
    args[0].ChangeDutyCycle(0)

if __name__ == "__main__":
    diffuser_pin = 33               # PWM pin for diffuser pump
    fan_pin = 12                    # PWM pin for PC fan
    GPIO.setwarnings(False)         # disable warnings
    GPIO.setmode(GPIO.BOARD)        # set pin numbering to RPi board
    GPIO.setup(diffuser_pin, GPIO.OUT)
    GPIO.setup(fan_pin, GPIO.OUT)

    diffuser_pwm = GPIO.PWM(diffuser_pin, 1000) # create diffuser PWM instance at 1kHz
    fan_pwm = GPIO.PWM(fan_pin, 1000)           # create fan PWM instance at 1kHz
    diffuser_pwm.start(0)                       # start diffuser PWM at 0%
    fan_pwm.start(0)                            # start fan PWM at 0%

    # for convenience, script also accepts IP and port as arguments (default 0.0.0.0 and 5005)
    parser = argparse.ArgumentParser()
    parser.add_argument("--ip",
    default="0.0.0.0", help="The ip to listen on")
    parser.add_argument("--port", type=int, default=5005, help="The port to listen on")
    args = parser.parse_args()

    # configure OSC dispatcher
    dispatcher = dispatcher.Dispatcher()
    dispatcher.map("/pi/cue/J1115/start", run_diffuser, diffuser_pwm)
    dispatcher.map("/pi/cue/J1120/start", run_fan, fan_pwm)

    # create OSC server
    server = osc_server.ThreadingOSCUDPServer((args.ip, args.port), dispatcher)

    print("{0}: serving on {1}".format(socket.gethostname(), server.server_address), flush=True)
    # run server until program close
    server.serve_forever()
