using Microsoft.Extensions.Logging.Console;

namespace JCClock.Common.Logging
{
    /// <summary>
    /// Data container class representing the options provided to FlatConsoleFormatter.
    /// </summary>
    public class FlatConsoleFormatterOptions : ConsoleFormatterOptions
    {
        // Not currently used. Retaining as a template for how to send options to the formatter.
        public bool DisableColors { get; set; }

        public FlatConsoleFormatterOptions()
        {
            DisableColors = false;
        }
    }
}