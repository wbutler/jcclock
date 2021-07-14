---
layout: post
title:  "Phrase Generation"
date:   2021-07-13 19:45:00 -0700
categories:
permalink: /2021-07-13-phrase-generation/
---
One of [the goals of this project that I stated]({{site.baseurl}}{% post_url 2021-04-11-starting-out %}) at the beginning is to automatically generate the front-panel letter layout. Before we can start seriously exploring that problem, we need the ability to generate a list of desired words and phrases that the device needs to display.

There are two categories of phrases that the clock needs to display:

- *Time phrases* are the ordinary output that tells the time, e.g. "IT IS FIVE MINUTES PAST SIX OCLOCK". This is the vast, vast majority of what the device will need to output.
- *Special phrases* are fun extra output for unusual circumstances, like "HAPPY BIRTHDAY *(owner's name)*" or "HAPPY NEW YEAR".

The special phrases don't require any generation logic--at this stage of the project, I'll just write a raw text file of input lines. If I decide to make a thousand of these things, I can re-evaluate.

The time phrases are the focus of this first bit of work. We're not displaying seconds, so there are only 1,440 possible outputs from 00:00 to 23:59. We need:

- A list of the desired output phrases that forms the input for our front-panel layout generator
- A mapping from tuples of [hour, minute] -> output phrase that will greatly simplify the writing of the device-side firmware in the future.

We ought to be able to generate both of those things automatically with only a few inputs. How do we figure out the total list of phrases we need to display?

## Time Phrases

If a friend asks you the time, you look at your watch, and it says "2:35", you might reasonably respond:

"It's two thirty-five."

But that's not too interesting. Almost all the words are the names of numbers, so it would be nice if we could add some variety. It's also not very impressive as a word clock, because it's pretty easy to generate. An equally correct and more colloquial way to state it is:

"It's twenty-five minutes till three."

This has the added benefit of saving words in the long run. We don't need to express the words "forty" and "fifty" in this scheme, so we'll save some space. That gives us the general form of:

```
IT'S <minute value> MINUTES [PAST|TO] <hour value>
```

The apostrophe is messy, so we'll make it "IT IS" rather than "IT'S". I'll also add "OCLOCK" onto the end, just because I like it. So that takes us to:

```
IT IS <minute value> MINUTES [PAST|TO] <hour value> OCLOCK
```

### Some Refinements

In casual English, some fractions of the hour have special names; we commonly say "it's a quarter to three", "it's a quarter till four", or "it's half past seven". It would be nice to add those cases, so it gives us some exceptions to the previous rule of the form:

```
IT IS A QUARTER [PAST|TO] <hour value> OCLOCK
IT IS HALF PAST <hour value> OCLOCK
```

We also have special names for certain hours. "Noon" and "midnight" are more evocative than "twelve o'clock", so we should support those as well:

```
IT IS <minute value> MINUTES [PAST|TO] MIDNIGHT
```

Of course, there are the combination cases where we need the ability to say "IT IS A QUARTER PAST NOON", etc, so we get to:

```
IT IS [<numeric minute> MINUTES|A QUARTER|HALF] [TO|PAST] [<numeric hour> OCLOCK|NOON|MIDNIGHT]
```

[A first implementation of this is here]({{site.data.globals.projectgithubroot}}blob/a19deeca9ac1dc5b254d03175d89931efdb2a291/src/software/PhraseGenerator/Program.cs#L83-L96) in the phrase generator code. 