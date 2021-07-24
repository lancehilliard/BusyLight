# BusyLight 🟥 👍

If you want to _automatically_ and _visually_ alert others nearby that you're busy, then BusyLight can help.

[![CI](https://github.com/lancehilliard/BusyLight/actions/workflows/main.yml/badge.svg)](https://github.com/lancehilliard/BusyLight/actions/workflows/main.yml)

## Use Case

You're at your desk, using one or more devices (desktop, laptop, mobile[1], etc) to perform work. You want to automatically indicate, to anyone nearby, to avoid interrupting you. BusyLight monitors your activity on your devices and lights up your [BlinkStick](https://www.blinkstick.com) automatically when you're busy.

BusyLight was built for your [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square). And, so far, it only tracks voice/video calls (the first two LEDs light up when software accesses[2] your microphone). What other BlinkStick products would you like it to support? What other work/activity would you like it to track?

## Prerequisite

BusyLight supports Windows 10 1903 or greater.

## Devices

Run the BusyLight application on any Windows computer whose work should inform your BlinkStick. Run the BusyLight application on any *one* Windows computer connected to your BlinkStick. If that involves separate computers (or several computers), great. If those are the same computer, that's fine, too.

## Requirements

* A [BlinkStick Square](https://www.blinkstick.com/products/blinkstick-square) must be connected to at least one machine running BusyLight.
* An [AMQP URL](https://github.com/lancehilliard/BusyLight/wiki/Messaging) must be specified on every machine running BusyLight.
* Some [configuration](https://github.com/lancehilliard/BusyLight/wiki/Configuration) must first be specified on every machine running BusyLight.

## Running the Software

See [Running the Software](https://github.com/lancehilliard/BusyLight/wiki/Running-the-Software) to get started.

### Notes
* [1] Are you working from your mobile phone, too? Check our [Mobile wiki page](https://github.com/lancehilliard/BusyLight/wiki/Mobile) for ideas!
* [2] Microphone mute/unmute is not tracked. Illumination occurs for the entire call, muted or not.
