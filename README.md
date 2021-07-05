# BusyLight

If you want to _automatically_ and _visually_ alert others nearby that you're busy, then BusyLight can help. The use case is a single desk with (potentially) multiple computers -- and maybe a mobile phone?[1] -- which are all intermittently used to perform work that, ideally, wouldn't be interrupted by others nearby, such that one light might convey "busy", no matter which machine was driving the work.

So far, it only tracks calls (active/ongoing voice and video calls, to be precise; what else would you like it to track?).

BusyLight is two Windows services which work in concert to illuminate a [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square) whenever your microphone is being accessed by any app on your computer(s).

## BusyLight.ActivityLoggingService

This service requires Windows 10 1903 or greater and detects when your microphone is being accessed. Run it on any computer(s)[1] from which you join calls.

## BusyLight.LightService

This service illuminates your BlinkStick when microphone access[2] is detected. Run it on the Windows computer attached to the [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square). It will automatically illuminate the first two LEDs on the device for the duration of your call.

## Requirements

* LightService expects a [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square) plugged into the machine.
* Both services require a free [restdb.io](https://restdb.io/) database with a collection named "[Activities](https://github.com/lancehilliard/BusyLight/wiki/Activities)".
* Some [configuration](https://github.com/lancehilliard/BusyLight/wiki/Configuration) is required for each of the two Windows services.

### Notes
* [1] Are you working from your mobile phone, too? Check our [Mobile wiki page](https://github.com/lancehilliard/BusyLight/wiki/Mobile) for ideas!
* [2] Microphone mute/unmute is not tracked. Illumination occurs for the entire call, muted or not.
