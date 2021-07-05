# BusyLight

BusyLight is two Windows services which work in concert to signal to others nearby that you are presently on a call, by illuminating a [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square) while your microphone is being accessed by a Desktop[1] app. It suits the scenario wherein you're regularly using one or more computers at the same workstation to hold calls, and you want to _automatically_ and _visually_ share that state with others nearby.

## BusyLight.ActivityLoggingService

This requires Windows 10 1903 or greater and detects when your microphone is being accessed. Run it on any computer[2] from which you join calls.

## BusyLight.LightService

This illuminates your BlinkStick when microphone access (muted or not) is detected. Run it on the machine having the [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square). It will automatically illuminate the first two LEDs on the device for the duration of your call.

## Requirements

* The code expects a [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square) plugged into the machine running LightService.
* Both services require a free [restdb.io](https://restdb.io/) account having a database with an [Activities collection](https://github.com/lancehilliard/BusyLight/wiki/Activities).
* Some configuration is required.

### Notes
* [1] Pull requests which add UWP app support are invited.
* [2] Are you joining your calls via your Android phone? Maybe [Tasker](https://github.com/lancehilliard/BusyLight/wiki/Tasker) can help!
