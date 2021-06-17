using Microsoft.Extensions.Logging;

namespace JCClock.Common.Logging
{
    public static class ConsoleLoggerFactory
    {
        /// <summary>
        /// Generates an ILoggerFactory that can supply an ILogger that writes simple output to the console.
        /// </summary>
        public static ILoggerFactory Get()
        {
            return LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole(options =>
                    {
                        options.FormatterName = FlatConsoleFormatter.FormatterName;
                        options.LogToStandardErrorThreshold = LogLevel.Trace;
                    })
                    .AddConsoleFormatter<FlatConsoleFormatter, FlatConsoleFormatterOptions>(options =>
                    {
                        options.DisableColors = true;
                        // N.B. that scopes aren't currently implemented in FlatConsoleFormatter.
                        options.IncludeScopes = false;
                        options.TimestampFormat = "HH:mm:ss ";
                    });
            });
        }
    }
    
}
