---
layout: post
title:  "Starting Out"
date:   2021-04-11 15:20:23 -0700
categories:
permalink: /2021-04-11-starting-out/
---
These entries will serve as the log\notebook for a project to construct a custom [word clock](https://hackaday.com/tag/word-clock) in a way that's technically fun and compelling. Which is to say, I'll overcomplicate some aspects if it gives me the opportunity to do something interesting that I might otherwise avoid due to cost or complexity.

### Requirements for Finished Product
- Should function as a reasonable clock:
	- Keeps time as well as a consumer clock radio.
	- Timekeeping persists across power disconnect.
	- Doesn't require internet access\GPS to do the above.
- Basic interaction should be possible via on-device buttons.
- Runs on wall power.
- We should generate a new, custom layout for the front panel rather than grab somebody else's, because...
- It should support some custom messages, e.g. "Happy Birthday (owner's name)", "Merry Christmas (owner's name)", etc.
  - Which implies knowing date as well as time.
- It's likely going to run in a child's bedroom, so it should be visible in daylight but not so obtrusive at night that it's disruptive to sleep. Adaptive brightness?
- Letters should have variable colors, i.e. RGB LED's or similar.
- Support for automation\debug\complex commands via serial comms.

### Implementation Choices
- I'd like to have the option to write firmware in C++.
- We won't use a Raspberry Pi, Arduino, or similar just because I want to do lower-level microcontroller dev.
- We'll use an ARM microcontroller, since I'm interested in some experience on the platform.
- The idea of manually arranging the words seems like needless toil. I'm going to try writing a util to auto-generate the layout.

### Constraints
- I don't have CNC tools in my home shop, and as of this writing, COVID is still decently serious. I'm assuming I'll need to use remote, on-demand fab for truly complex parts like PCB's and routed letters on a front panel.