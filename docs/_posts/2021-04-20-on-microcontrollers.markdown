---
layout: post
title:  "On Microcontrollers"
date:   2021-04-20 21:33:10 -0700
categories:
permalink: /2021-04-20-on-microcontrollers/
---
The next big decision is to pick a microcontroller platform for the project. I don't have a ton of MCU experience, but I'm not starting completely from scratch. I've written toy firmware for a handful of development boards and seen two major projects through to the end.

### Previous Experience
The first was a two-digit 7-segment numeric sign. This was, I think, > 10 years ago. Sparkfun sold these super big 6" LED digits (which, wow, [they actually still carry](https://www.sparkfun.com/products/8530)), and I made up a little board around a PIC 16F627 and a couple of LED driver chips. It would listen for ASCII digits coming in over the UART and repeat them out to the display, so you could hook it up to a PC with an old-fashioned serial cable and display a number.

It was my first time doing any real MCU development, and I mostly wanted to get my head around how to write a piece of code on a PC that had any kind of effect on a custom device. I taught myself enough EAGLE to get a real PCB made up, and the whole thing actually worked pretty well. It still has a place of honor on the shelves in my shop.

The second major MCU project I've done was at work. At the time, I was assigned to a project building a consumer hardware device, and my particular team was working on custom factory stations that would run in our manufacturing facility. Our product had some sensors that had to be tested and calibrated, so we ended up building a station in which one PC would communicate with multiple sensors via a custom board containing a PIC32.

A real microcontroller dev did the heavy lifting to get us started, but I contributed a handful of small features to that firmware and owned the PC side and the station software all-up. I can't write a ton about it because it's proprietary, but it remains my favorite thing I've ever built or worked on.

### Moving On
So what experience I have is in Microchip-land, but [I said in a previous post]({{site.baseurl}}{% post_url 2021-04-12-first-decisions %}) that I don't want to use Microchip on this project. There are a few reasons for that, most notably that I've found their dev tools to be mediocre and their first-party programmers tend to be awfully expensive. Really, though, I'm just looking to do something new and teach myself a new platform.

As I've said before, there is [a huge array of options available](https://en.wikipedia.org/wiki/List_of_common_microcontrollers). The constraints I've put on myself are:

- Not a Raspberry PI\small Linux box. I'd like it to be a microcontroller project, not a lightweight PC project.
- Use an ARM core. I've never written low-level ARM code and I'd like to get some experience there.
- Not an Arduino or similar platform. I want to be a bit closer to the hardware than that and I don't mind the additional effort. On the contrary, it's really the point of things.
- Something with a toolchain that supports C++, which I don't think is a substantial constraint on modern ARM MCU's, but is worth stating.
- Finally, something hobbyist-friendly. If it costs $1K to get started with tools and a dev board, or if I can't buy and program chips in small quantity, it's not for me.

Honestly, this doesn't constrain the space very much; there's still a wealth of things available that look pretty good. Having looked around, this all feels a little bit like spinning a globe and throwing a dart, so...

### STM32

I think I'm going to go with STMicroelectronics's line of ARM-based MCU's. There's a few things that pulled me in.

- Their [first-party firmware tooling](https://www.st.com/en/development-tools/stm32cubeide.html) looks pretty reasonable, and it's based on GCC so I can throw it over the side if need be.
- What I *think* is [the full featured programmer and debugger](https://www.mouser.com/ProductDetail/STMicroelectronics/STLINK-V3SET?qs=sGAEpiMZZMu3sxpa5v1qrhWKVY0UyKH45NIjGrvpIMc%3D) is super reasonably priced.
- Although it's not the PIC mothership, the community on the web seems reasonably deep, with plenty for a dilettante like me to learn from.
- There seems to be a wide array of parts with every peripheral I can think of.
- Their [Nucleo line of eval boards](https://www.st.com/en/evaluation-tools/stm32-nucleo-boards.html) offers a lot of options with integrated programmer\debugger at very friendly prices.

The main drawback I can see to this selection is that they don't offer an ARM MCU in a friendlier format than QFP, but I suppose I'll have to be brave and teach myself SMT soldering.

I've put in an order for [this small Nucleo-L432KC development board](https://www.mouser.com/ProductDetail/STMicroelectronics/NUCLEO-L432KC?qs=%2Fha2pyFaduhRqNkTb7sttrZziBhsdYMt2li52DE18P72bnhyZXerPw%3D%3D) to take things for a spin. If I can get this board running and controlling a set of Neopixels, a lot of architecture questions will be settled and validated. I'm pretty excited to start writing some firmware!
