# BusyLight

If you want to _automatically_ and _visually_ alert others nearby that you're busy, then BusyLight can help.

So far, it only tracks calls (voice calls, to be precise). What else should it track? Leave a comment below, and don't forget to smash that "LIKE" bu-- oh wait... wrong platform.

BusyLight is two Windows services which work in concert to illuminate a [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square) whenever your microphone is being accessed by any app on your computer.

## BusyLight.ActivityLoggingService

This service requires Windows 10 1903 or greater and detects when your microphone is being accessed. Run it on any computer(s)[1] from which you join calls.

## BusyLight.LightService

This service illuminates your BlinkStick when microphone access[2] is detected. Run it on the Windows computer attached to the [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square). It will automatically illuminate the first two LEDs on the device for the duration of your call.

## Requirements

* LightService expects a [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square) plugged into the machine.
* Both services require a free [restdb.io](https://restdb.io/) account having a database with an [Activities](https://github.com/lancehilliard/BusyLight/wiki/Activities) collection.
* Some [configuration](https://github.com/lancehilliard/BusyLight/wiki/Configuration) is required.

### Notes
* [1] Are you joining your calls via your Android phone? Maybe [Tasker](https://github.com/lancehilliard/BusyLight/wiki/Tasker) can help!
* [2] Microphone mute/unmute is not tracked. Illumination occurs for the entire call.
