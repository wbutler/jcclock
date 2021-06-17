using Microsoft.Extensions.Logging;

using JCClock.Common.Logging;

namespace JCClock.TestApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            using (ILoggerFactory loggerFactory = ConsoleLoggerFactory.Get())
            {
                ILogger logger = loggerFactory.CreateLogger("Program");
                using (logger.BeginScope("Start a scope."))
                {
                    logger.Log("Hello, world!");
                }
            }
            return;
        }
    }
}