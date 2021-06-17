using Microsoft.Extensions.Logging;

namespace JCClock.Common.Logging
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Extension method for ILogger to implement simple printf-style log lines.
        /// </summary>
        public static void Log(this ILogger logger, string message, params object[] args)
        {
            logger.LogInformation(message, args);
        }
    }
}
