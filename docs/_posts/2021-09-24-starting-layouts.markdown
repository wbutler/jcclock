---
layout: post
title:  "Starting Layouts"
date:   2021-09-24 17:00:00 -0700
categories:
permalink: /2021-09-24-starting-layouts/
---
Let's get started on laying out letters on clock faces.

On this project, a clock layout is a rectangular grid of letters like this one:

`A B C D`  
`E F G H`  
`I J K L`

Words are displayed horizontally and read left-to-right, since we're only thinking about English. Vertically displayed words and non-rectangular layouts are out of scope for now.

For a phrase to be properly displayed, we require all the words, in order, with no line breaks or spaces inside a given word. Multiple spaces and line breaks between words are fine. So the layout above could display the phrases `BC EFG JK` and `ABCD GH` but not `IJ FG` or `KLM`.

I haven't decided on an algorithmic approach yet, but I do know that I'll want to be able to take some layout and evaluate whether or not it can display a given set of phrases. That will be useful for checking whether or not a generation algorithm is finished and\or executed properly.

We'll also want to know, even if a layout can't completely display a set of phrases, _how close_ is it to being right? Is it missing just a handful of letters, or is every phrase broken? In particular, if we're running our generation algorithm and we make a single change, did it get us closer to a solution or not? Knowing that will be incredibly helpful if we want to use e.g. a genetic algorithm that will need some sort of fitness function.

### Internal Representation

Internally, I'm storing layouts as a flat string with the lines separated by `|` characters. In this format, the layout above is stored as:

`"ABCD|EFGH|IJKL"`

The main advantage of this scheme is that we can use simple substring or regex functions to check matches of words and phrases. I'm very sure I borrowed it from another project but I can't think of where. Apologies for the uncredited lift.

### Checking Phrases

For every phrase in our input list we define a simple metric of match quality, which we calculate as

_characters matched / characters in phrase_

So a full, valid match for any phrase has quality of 1.0. The total quality of the overall layout is the sum of all the phrase qualities, and the target quality for layout generation is just the count of phrases.

To test how well a certain layout can match a phrase, the procedure is straightfoward:

```csharp
public PhraseMatch Evaluate(LayoutPhrase phrase)
{
    int wordIndex = 0;
    int layoutIndex = 0;
    WordMatch[] matches = new WordMatch[phrase.Words.Length];

    while(layoutIndex < LayoutBuffer.Length && wordIndex < phrase.Words.Length)
    {
        int matchLength;
        try
        {
            matchLength = EvaluateWordAtLocation(phrase.Words[wordIndex], layoutIndex);
        }
        catch(IndexOutOfRangeException)
        {
            throw;
        }

        // If the match as this location is better than what we had, record it.
        bool isFullMatch = matchLength == phrase.Words[wordIndex].Length;
        if (matches[wordIndex] == null || matches[wordIndex].Length < matchLength)
        {
            matches[wordIndex] = new WordMatch(layoutIndex, matchLength, isFullMatch, phrase.Words[wordIndex]);
        }

        if(isFullMatch)
        {
            // If we completely matched the word at this location, search for the next word.
            wordIndex++;

            // Advance our search index the length of the word plus a space.
            layoutIndex += matchLength + 1;
        }
        else
        {
            // We didn't find a complete match here. Go on to the next character.
            layoutIndex++;
        }
    }

    return new PhraseMatch(phrase, matches);
}
```

[See it in the repo]({{site.data.globals.projectgithubroot}}blob/37efaa89cfc8333be6989a9feb5cd347f86a1470/src/software/LayoutGenerator/Layout.cs#L106-L151) for more details. The `PhraseMatch` class keeps track of in the layout each word is found so we can check correctness and see what the phrase will look like on our layout.

To exercise all of this, I wrote a test utility that with the following layout:

`I T A I S A A A`  
`F I V E A A A A`  
`A A A T E N A A`  
`A A A A A A A A`

And then reads in the .json file [output by the phrase generator]({{site.baseurl}}{% post_url 2021-07-13-phrase-generation %}) and computes how well the layout can display each phrase:

```
Loaded C:\Users\Will\source\repos\jcclock\src\software\PhraseGenerator\bin\Debug\net5.0\phrases.json
156 phrases in input.
I T A I S A A A
F I V E A A A A
A A A T E N A A
A A A A A A A A

Best match for IT IS MIDNIGHT:
Quality: 0.333
I T . I S . . .
. . . . . . . .
. . . . . . . .
. . . . . . . .

Best match for IT IS FIVE MINUTES PAST MIDNIGHT:
Quality: 0.296
I T . I S . . .
F I V E . . . .
. . . . . . . .
. . . . . . . .

Best match for IT IS TEN MINUTES PAST MIDNIGHT:
Quality: 0.269
I T . I S . . .
. . . . . . . .
. . . T E N . .
. . . . . . . .

Best match for IT IS A QUARTER PAST MIDNIGHT:
Quality: 0.208
I T . I S . A .
. . . . . . . .
. . . . . . . .
. . . . . . . .

<snip>
```

[This initial checkin]({{site.data.globals.projectgithubroot}}tree/37efaa89cfc8333be6989a9feb5cd347f86a1470/src/software/LayoutGenerator) doesn't actually generate layouts, but it does rough out our basic data structures and give us the evaluation functions we need to know if our algorithm is successful. I'll move on to make a first attempt at actually generating layouts.