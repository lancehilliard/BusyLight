# BusyLight 🟥 👍

If you want to _automatically_ and _visually_ alert others nearby that you're busy, then BusyLight can help.

[![CI](https://github.com/lancehilliard/BusyLight/actions/workflows/main.yml/badge.svg)](https://github.com/lancehilliard/BusyLight/actions/workflows/main.yml)

## Use Case

You're using one or more devices (Windows desktop and/or laptop, mobile[1], etc) to perform work. You sometimes want to automatically signal "please don't interrupt me" to anyone nearby. BusyLight monitors the activity of your various devices and, during their "busy" moments, illuminates your single [BlinkStick](https://www.blinkstick.com).

BusyLight was built for a [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square). And, so far, it only tracks voice/video calls (the first two LEDs light up when software accesses[2] your microphone). What other BlinkStick products would you like it to support? What other work/activity would you like it to track?

## Prerequisite

BusyLight supports May 2019's Windows 10 1903 or greater. It exploits system data that debuted in that build.

## Devices

BusyLight runs on the machine connected to your BlinkStick, and also on any other machine(s) you're working on. That _can_ involve only a _single_ device, if you aren't using multiple devices, but BusyLight's strength lay in controlling a single BlinkStick based on the activity of _multiple_ devices.

## Requirements

* A [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square) must be connected to at least one Windows machine running BusyLight.
  * otherwise, BusyLight will display an approximate simulation of BlinkStick activity
    * (good for testing, if you're still debating the device purchase)
* Some [configuration](https://github.com/lancehilliard/BusyLight/wiki/Configuration), including an [AMQP URL](https://github.com/lancehilliard/BusyLight/wiki/Messaging), must  be specified on every machine running BusyLight.

## Running the Software

See [Running the Software](https://github.com/lancehilliard/BusyLight/wiki/Running-the-Software) to get started.

### Notes
* [1] Are you working from your mobile phone, too? Check our [Mobile wiki page](https://github.com/lancehilliard/BusyLight/wiki/Mobile) for ideas!
* [2] Microphone mute/unmute is not tracked. Illumination occurs for the entire call, muted or not.
