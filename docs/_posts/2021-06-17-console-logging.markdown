---
layout: post
title:  "Console Logging"
date:   2021-06-17 22:55:00 -0700
categories:
permalink: /2021-06-17-console-logging/
---
With the approach to driving the display figured out, I thought I'd begin some of the PC-side software. [I wrote way back when]({{site.baseurl}}{% post_url 2021-04-11-starting-out %}) that it would save some toil if I could write a util to automatically generate the front-panel word layout, so that's a good place to start.

That led me down a side path that's worth documenting. It's always nice to decouple dependencies if you can, and I'd rather not have direct calls to print to the console scattered all over the code; it makes things difficult if we later want to plug in a more sophisticated logging pipeline.

In .NET, [ILogger](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.ilogger?view=dotnet-plat-ext-5.0) is a common interface that provides integration with several different logging sinks and offers API's for tracking execution spans and the like. It has built-in sinks to output data to the console in a few different formats, but they add decoration, color, and line-splitting to your output. The default console logging module yields output like this:

{% include image-block.html file="consoleloggingcomplex.png" caption="Default console logging behavior for ILogger in .NET 5" align="center" %}

The "info" string represents the log level, one of `{critical, error, warning, information, debug, trace}`. `Program` is the name of the class instantiating the logger object.

This isn't bad, necessarily, but it's very chatty. There's config so you can remove the line break, but the default classes don't let you trim off all the decoration and get true printf-style behavior.

.NET 5 allows for the implmentation of [custom console log formatting](https://docs.microsoft.com/en-us/dotnet/core/extensions/console-log-formatter). I found it touchy to get just right, but [this reference sample](https://gist.github.com/maryamariyan/8fdf800318f61b1244b42c185b83b179) from Microsoft's Maryam Ariyan got me where I wanted to go.

The result is all the API and pluggability benefits of ILogger with the simplicity of basic console output:

{% include image-block.html file="consoleloggingsimple.png" caption="Default console logging behavior for ILogger in .NET 5" align="center" %}

I'll use [the resulting logging library]({{site.data.globals.projectgithubroot}}tree/333d9508f22d59f6b2460d4b6cc2a167ee6a6c06/src/software/Common/Logging) for the PC-side software on this project.