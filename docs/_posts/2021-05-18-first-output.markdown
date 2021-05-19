---
layout: post
title:  "First... Output?"
date:   2021-05-18 20:58:00 -0700
categories:
permalink: /2021-05-18-first-output/
---
I wired up my Neopixel stick to the Nucleo board, seen here with comically huge input capacitor:

{% include image-block.html file="neopixelbreadboard.jpg" caption="Neopixel stick connected to breadboard" align="center" %}

Incidentally, whoever at Adafruit that decided to lay out the pads at the exact pitch that lets you solder on pin headers and stick it on a breadboard is brilliant.

I hooked everything up and, just to see what would happen, ran my firmware to output the test PWM signal of 67% @ 800 kHz. If I read the datasheet correctly, that should have been equivalent to spamming 1's to the serial input line.

Expecting all white, I saw... nothing. No problem, I thought, it was worth trying. I haven't fine-tuned the timing, so I'll test it out for real some other time.

I happened to disconnect the microcontroller power before the LED power; when I did, the lights came on so bright that I jumped in my seat. They stayed on steady until I shut off the power supply. What was happening?

Remember that these LED's consume a stream of serial data terminated by a RESET sequence of > 50μs of low voltage:

{% include image-block.html file="ws2812protocoltiming.png" caption="Timing specification for WS2812 control frame" align="center" %}

When I disconnected power to the microcontroller, the serial data level dropped to zero, which the WS2812 controllers interpret as a RESET.

It makes sense, but if it's in the datasheet, I didn't absorb it on my first read through. The RESET doesn't just terminate the data stream--the LED's *won't act on the input until they get the RESET*. They were sitting there the whole time with that stream of spammed 1's in memory, but didn't actually turn on until they received that terminating segment.

Which means that the raw PWM signal can actually give us a super-hacky way to blink the entire light bar, if we make a loop as follows:

1. Set PWM to 67% (spam 1's)
2. Set PWM to 0 (RESET, lights turn on)
3. Set PWM to 33% (spam 0's)
4. Set PWM to 0 (RESET, lights turn off)

[Firmware code is here]({{site.data.globals.projectgithubroot}}tree/bd1f0106c9d8986eddfb6250b58f55243f550614/src/firmware/nucleo/roughoutput), and here it is in action. As you can see, it outputs a *ton* of light.

{% include vimeo-player.html id=551823444 %}

The microcontroller can light up the LED's! It's not really proper control of the component, since we can't do colors and we aren't individually controlling them, just blasting data to the whole array. But it proves out that the PWM signal is the right frequency and the right duty cycles. It also proves that the physical connections and power supply are reasonable, so I'll take it. Varying the duty cycle for proper control is up next.