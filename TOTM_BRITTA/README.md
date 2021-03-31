# Theater of the Mind
### Multisensory Chair Controller Software

This repository contains python code and linux configuration files for the Theater of the Mind attic scene multisensory controller, which is embedded in armchairs within the set. As configured, the controller can activate a fan embedded in the armchair and/or a micropump-based scent diffuser in response to OSC messages received on port `5005`.

The scent diffuser is operated with the following OSC message, with argument duration given in seconds. A good ballpark value is 20, but should be tuned to the length of the internal tubing and desired intensity.

`/pi/cue/J1115/start duration`

The fan is operated with the following OSC message, with arguments duration given in seconds and wind_intensity given in the range 10-100. The wind_intensity argument is optional and defaults to 100.

`/pi/cue/J1120/start duration wind_intensity`

The software is designed to run on a Raspberry Pi computer, and depends on Python3 as well as external packages that are available from the pip package manager: RPi.GPIO and pythonosc. The software assumes the provided electronics hardware is mounted on the Raspberry Pi host.

The controller software, `osc_server.py`, can be managed either as a systemd service or by Supervisord, a third party process control system. Supervisord enables remote start/stop/restart as well as monitoring of the standard output, all via http on port 9001: (`http://ip_address:9001`).

Gershon Dublon, slow immediate LLC.
