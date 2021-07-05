using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace JCClock.Common.Logging
{
    /// <summary>
    /// Console log formatter for sending messages from an ILogger to the console. Writes messages with no
    /// decoration of level and no line breaks to supply near-equivalent functionality to System.Console.
    /// 
    /// Adapted with thanks from https://gist.github.com/maryamariyan/8fdf800318f61b1244b42c185b83b179. 
    /// </summary>
    public class FlatConsoleFormatter : ConsoleFormatter, IDisposable
    {
        public static readonly string FormatterName = "FlatConsoleFormatter";

        private IDisposable _optionsReloadToken;

        public FlatConsoleFormatter(IOptionsMonitor<FlatConsoleFormatterOptions> options)
            // case insensitive name for the formatter
            : base(FlatConsoleFormatter.FormatterName)
        {
            ReloadLoggerOptions(options.CurrentValue);
            _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        }

        private void ReloadLoggerOptions(FlatConsoleFormatterOptions options)
        {
            FormatterOptions = options;
        }

        public FlatConsoleFormatterOptions FormatterOptions { get; set; }
        public void Dispose()
        {
            _optionsReloadToken?.Dispose();
        }

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
        {
            string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
            if (logEntry.Exception == null && message == null)
            {
                return;
            }

            if (logEntry.Exception == null)
            {
                textWriter.WriteLine(message);
            }
            else
            {
                textWriter.WriteLine(message + " " + logEntry.Exception.ToString());
            }
        }
    }
}