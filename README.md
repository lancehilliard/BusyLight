# BusyLight

If you want to _automatically_ and _visually_ alert others nearby that now might not be the best time to interrupt you, then BusyLight can help.

So far, it only tracks ongoing calls. What else should it track? Leave a comment below, and don't forget to smash that "LIKE" bu-- oh wait... wrong platform.

BusyLight is two Windows services which work in concert to illuminate a [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square) whenever your microphone is being accessed by a Desktop[1] app on your computer.

## BusyLight.ActivityLoggingService

This service requires Windows 10 1903 or greater and detects when your microphone is being accessed. Run it on any computer(s)[2] from which you join calls.

## BusyLight.LightService

This service illuminates your BlinkStick when microphone access[3] is detected. Run it on the Windows computer attached to the [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square). It will automatically illuminate the first two LEDs on the device for the duration of your call.

## Requirements

* The code expects a [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square) plugged into the machine running LightService.
* Both services require a free [restdb.io](https://restdb.io/) account having a database with an [Activities](https://github.com/lancehilliard/BusyLight/wiki/Activities) collection.
* Some [configuration](https://github.com/lancehilliard/BusyLight/wiki/Configuration) is required.

### Notes
* [1] Pull requests which add UWP app support are invited.
* [2] Are you joining your calls via your Android phone? Maybe [Tasker](https://github.com/lancehilliard/BusyLight/wiki/Tasker) can help!
* [3] Microphone mute/unmute is not tracked. While the microphone is accessed at all, illumination occurs.
