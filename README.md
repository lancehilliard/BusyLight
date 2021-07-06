# BusyLight

If you want to _automatically_ and _visually_ alert others nearby that you're busy, then BusyLight can help.

## Use Case

The use case is your single desk with one or more computers (and maybe a mobile device?[1]) -- all intermittently involved in work that, ideally, wouldn't be interrupted by others nearby, such that a single and automatic light might sometimes convey "busy", no matter which device was driving the work.

So far, it only tracks calls (active/ongoing voice and video calls, to be precise; what else would you like it to track?).

## Services

BusyLight is two Windows services which work in concert to illuminate a [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square) whenever your microphone is being accessed by any app on your computer(s).

### BusyLight.ActivityPublisher

This service requires Windows 10 1903 or greater and detects when your microphone is being accessed. Run it on any computer(s)[1] from which you join calls.

### BusyLight.LightSubscriber

This service illuminates your BlinkStick when microphone access[2] is detected. Run it on the Windows computer attached to the [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square). It will automatically illuminate the first two LEDs on the device for the duration of your call.

## Requirements

* The ActivityPublisher service expects a [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square) plugged into the machine.
* Both services expect a [message queue](https://github.com/lancehilliard/BusyLight/wiki/Messaging).
* Both services require some [configuration](https://github.com/lancehilliard/BusyLight/wiki/Configuration).

## Running the Software

See [Running the Software](https://github.com/lancehilliard/BusyLight/wiki/Running-the-Software) to run the services locally.

### Notes
* [1] Are you working from your mobile phone, too? Check our [Mobile wiki page](https://github.com/lancehilliard/BusyLight/wiki/Mobile) for ideas!
* [2] Microphone mute/unmute is not tracked. Illumination occurs for the entire call, muted or not.
