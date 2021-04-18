---
layout: post
title:  "Driving LED's"
date:   2021-04-17 21:01:10 -0700
categories:
permalink: /2021-04-17-driving-leds/
---
On this project, the primary display will consist of individually controllable RGB LED's. Per [the notes in a previous post]({{site.baseurl}}{% post_url 2021-04-12-first-decisions %}), I expect that I'll need to control > 150 display elements.

### The Lower Bound
My first instinct had been to reach for [something like this](https://www.mouser.com/new/nxp-semiconductors/nxp-pca9956b-driver/), which allows for the control of multiple elements over I2C. It would likely work, but for our minimum display size of 150 LED's\450 elements, we get to a minimum of 19 IC's just driving the display.

At low quantity, Mouser wants $2.85 for this part. We can look around and find something cheaper, but we won't do better by an order of magnitude. For this approach, figure 19 x $2.85 = $54.15 for driver IC's and $0.95 * 150 = $142.40 for bare LED's. Near as makes no difference $200. Of course, that's having done no work on more thoughtfully and inexpensively sourcing the parts, so I'll guess that the naive approach costs $175.

That approach is also bare-bones from a mechanical perspective. I'd have to figure out how to fix all of those LED's in some sort of frame and then hand-wire and hand-solder all of them back to the mainboard. Bound to be a fiddly and error-prone exercise, so I'm considering it well and truly an option of last resort.

### A Better Approach
[This build by Chloe Kuo](https://www.youtube.com/watch?v=SXYwSN6mX_Q) is the first of many I found that relies on [Adafruit's Neopixel line of products](https://www.adafruit.com/index.php?main_page=category&cPath=168&gclid=Cj0KCQjw6-SDBhCMARIsAGbI7Ug89EFew8ob_vperXs_FeVsjbVweQATGDkeiFyJ05-7DyPj8BzE2DkaAvufEALw_wcB). "Neopixel" is Adafruit's brand name for their series of boards and flex-tape products that rely on WS2812 LED's.

The WS2812's have four pins: one each for power and ground, one for serial data in, and one for serial data out. Their internal drivers speak a proprietary serial protocol that's very timing-sensitive, so they'll be more challenging to integrate than an I2C or SPI-based driver chip. The huge benefit is that they can be daisy-chained via the data in\out ports.

Each LED will listen to control data coming in, take the first complete data frame for itself, and then pass the rest of the bits through untouched. The next LED in the chain will take the new first frame for itself and pass the rest along. You can chain an arbitrary number together to create a long strip of individually addressable LED's with a single digital pin.

They also seem to work well mechanically. [The reel format](https://www.adafruit.com/product/1461) can be cut and re-soldered to allow for custom sizes, so I won't be constrained to a grid that's a multiple of a certain size, and I'll only have to devise mounting and wiring for a handful of strips, rather than hundreds of individual elements.

Finally, it's substantially more cost-effective than the naive appoach. The 3m length, at 60 LED's\meter, contains 180 LED's for $74.85. Since it has drivers built-in, it's less than half of the total cost, which is great.

The only real drawback is that a product like this constrains our grid size, since the distance between the LED's, and therefore the letters on our clock, is predetermined. I think that's worth the tradeoffs, though. Adafruit offers 60/m and 32/m, which translate to a grid size of 0.65" and 1.23", so there are still options.

I think this is sufficiently compelling that I'll order a small Neopixel board to prototype against, and I can move on to figuring out a microcontroller approach.