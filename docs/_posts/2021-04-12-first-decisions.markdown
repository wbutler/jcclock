---
layout: post
title:  "First Decisions"
date:   2021-04-13 20:52:00 -0700
categories:
permalink: /2021-04-13-first-decisions/
---
There are a few broad selections to make to begin putting together a design.

### Microcontroller Selection
Deciding to use an ARM microcontroller still leaves [a ton of options](https://en.wikipedia.org/wiki/List_of_common_microcontrollers). I've already decided not to go with Microchip, since I've done major work on their MCU's in the past; that still leaves lots of choices. I'm tabling this for now and looking at choices.

### Driving the Display
Looking at [a well-executed example of a classic word clock design](https://hackaday.com/2021/02/21/this-slimline-word-clock-uses-laser-etching-to-keep-things-simple/), there's a strong lower bound on our display size at 10x11, since this unit doesn't implement the holiday messages or owner's name.

Adding characters for "Merry Christmas" and "Happy Birthday", we need, *at minimum*, (10 x 11) + 5 + 9 + 5 + 8 = 137 LED's. With distinct RGB elements, that brings us to 411 individual LED's to power and control. Realistically, we shouldn't give a serious look to solutions that control fewer than 150 RGB LED's or 450 distinct elements.

Obviously, this is a significant requirement, and will also drive cost. A [bare RGB LED from Sparkfun](https://www.sparkfun.com/products/11120) costs $0.95 in quantity in early 2021. Additionally, admittedly as a rank novice who's never really worked on a project like this, I can't think of a driver IC that can handle more than 24 channels. So there's some research needed here.

Finally, the power requirements will be nontrivial. I'm sure that by driving the individual elements at a low percentage of their rated output I can mitigate that, but that means I'll also need to devise a protection mechanism so I don't see sparks and smoke upon the inevitable mistakes.

The upshot of all this is that I can feel free to put a powerful microcontroller on the board. No reason to beat myself up to save a few dollars when I'm fated to spend quite a bit on the display.

### PC Software
The project will have a decent amount of software that doesn't run on the device itself, including:

- Some PC-side debugging\control utility for bringup and validation of the PCB and microcontroller firmware.
- The aforementioned layout generator tool, which will take the list of phrases we want to be able to display and output an optimal or at least reasonably compact arrangement of front panel letters.

This is a pretty easy choice. I have a lot of experience in C#, and although I'm looking to stretch myself on the hardware side of this project and potentially the layout generation algorithm, I'll go with something I understand well for the utility software.

I'll target .NET 5 because, why not, and to give it the best chance at having any kind of cross-platform story.
