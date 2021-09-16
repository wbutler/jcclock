using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using JCClock.Common;
using JCClock.Common.Logging;

namespace JCClock.PhraseGenerator
{
    class Program
    {
        // Caches the list of times we're actually going to display to the user.
        private static HashSet<TimeSpan> DisplayTimes;

        public static void Main(string[] args)
        {
            using (ILoggerFactory loggerFactory = ConsoleLoggerFactory.Get())
            {
                ILogger logger = loggerFactory.CreateLogger("PhraseGenerator");
                try
                {
                    PhraseGeneratorArguments config = PhraseGeneratorArguments.Parse(args);
                    ClockPhraseCollection outputCollection = new ClockPhraseCollection();

                    // Add custom phrases from a file, if any.
                    if(!String.IsNullOrEmpty(config.CustomPhraseFile))
                    {
                        if(!File.Exists(config.CustomPhraseFile))
                        {
                            throw new ArgumentException(String.Format("The file {0} does not exist.", config.CustomPhraseFile));
                        }

                        outputCollection.SpecialPhrases.AddRange(
                            File.ReadAllLines(config.CustomPhraseFile).
                            Select(str => CanonicalizePhrase(str)).
                            Where(str => !String.IsNullOrEmpty(str))
                            );
                    }

                    // Generate time phrases.
                    for (int hour = 0; hour <= 23; hour++)
                    {
                        for (int minute = 0; minute <= 59; minute++)
                        {
                            TimeSpan exactTime = new TimeSpan(hour, minute, 0);
                            TimeSpan mappedTime = GetMappedTime(exactTime, config.MinuteGranularity, config.MappingStrategy);

                            string timePhrase = CanonicalizePhrase(GenerateTimePhrase(mappedTime.Hours, mappedTime.Minutes));
                            logger.Log("{0:D2}:{1:D2} -> {2:D2}:{3:D2} -> {4}", hour, minute, mappedTime.Hours, mappedTime.Minutes, timePhrase);
                            outputCollection.TimePhrases.Add(new TimeDisplayMapping(hour, minute, timePhrase));
                        }
                    }
                    
                    // We burn a bit of extra disk by writing our output with nice formatting, but it doesn't matter in this application.
                    File.WriteAllText(config.OutputPath, JsonConvert.SerializeObject(outputCollection, Formatting.Indented));
                    logger.Log("Wrote output to {0}.", config.OutputPath);
                }

                // We hit this and do nothing (exit cleanly) if we failed to parse our command-line args.
                catch(CommandLineParseException)
                { }

                catch (Exception ex)
                {
                    logger.LogError(ex, "Global Exception.");
                }
            }
        }

        /// <summary>
        /// For a given input string, apply consistent formatting rules and return the result.
        /// </summary>
        private static string CanonicalizePhrase(string inputPhrase)
        {
            return inputPhrase.ToUpper().Trim();
        }

        /// <summary>
        /// For a given input combo of integer hour and minute, return the text string that represents that time.
        /// </summary>
        private static string GenerateTimePhrase(int hour, int minute)
        {
            // | PREFIX |                           MINUTE DELTA                        | PROXIMATE HOUR | HOUR SIGNIFIER |
            // |        | MINUTE DELTA MAGNITUDE | MINUTE SIGNIFIER | MINUTE DELTA SIGN |                |                |
            // |        |                        |                  |                   |                |                |
            // | IT IS  |         FIVE           |     MINUTES      |        TO         |      SIX       |     OCLOCK     |
            // | IT IS  |         TEN            |     MINUTES      |       PAST        |      NOON      |                |
            // | IT IS  |       A QUARTER        |                  |        TO         |    MIDNIGHT    |                |
            // | IT IS  |       A QUARTER        |                  |       PAST        |      NINE      |     OCLOCK     |
            // | IT IS  |         HALF           |                  |       PAST        |    MIDNIGHT    |                |
            // | IT IS  |                        |                  |                   |    MIDNIGHT    |                |

            string prefix = "it is";
            string minuteDeltaSign = "past";
            string minuteSignifier = "minutes";
            string hourSignifier = "oclock";
            bool specialHour = false;
            bool specialMinute = false;

            // Decide what string we're going to use for the hour. If it's later than half past,
            // round up the hour to the next value.
            string proximateHour = "";
            if (minute > 30)
            {
                hour++;
                minuteDeltaSign = "to";
            }

            // Use special strings for noon and midnight.
            if (hour == 0 || hour == 24)
            {
                proximateHour = "midnight";
                specialHour = true;
            }
            else if (hour == 12)
            {
                proximateHour = "noon";
                specialHour = true;
            }
            else
            {
                // Casual English doesn't use hours > 12, e.g. "fourteen o'clock"
                hour %= 12;

                // Get the hour string out of the translation table.
                proximateHour = NumberTextTranslator.Get(hour);
            }

            // If we used a string like noon or midnight, omit "o'clock".
            if (specialHour)
            {
                hourSignifier = "";
            }

            // Now decide on the string to represent minutes.
            string minuteDeltaMagnitude = "";
            // Use special strings for *:15, *:30, and *:45.
            if (minute == 15 || minute == 45)
            {
                minuteDeltaMagnitude = "a quarter";
                specialMinute = true;
            }
            else if (minute == 30)
            {
                minuteDeltaMagnitude = "half";
                specialMinute = true;
            }
            // If it's the top of the hour, don't write anything at all for the minute.
            else if (minute == 0)
            {
                specialMinute = true;
                minuteDeltaSign = "";
            }
            else
            {
                // At this point, do standard minute behavior.
                // If it's more than half past, we want to express the minute as the time left
                // until the top of the hour, e.g. "ten minutes until", not "fifty minutes after.'
                if (minute > 30)
                {
                    minute = 60 - minute;
                }

                // Get the string from the translation table.
                minuteDeltaMagnitude = NumberTextTranslator.Get(minute);
            }

            // No need to include the string "MINUTES" if we're doing "QUARTER PAST",
            // "HALF PAST", etc.
            if (specialMinute)
            {
                minuteSignifier = "";
            }
            // Adjust for singular minute if necessary.
            else if(minute == 1)
            {
                minuteSignifier = "minute";
            }

            string result = String.Format(
                "{0} {1} {2} {3} {4} {5}",
                prefix,
                minuteDeltaMagnitude,
                minuteSignifier,
                minuteDeltaSign,
                proximateHour,
                hourSignifier
                );

            // Collapse multi spaces to single space.
            result = String.Join(" ", result.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            // Capital letters only.
            result = result.ToUpper();

            return result;
        }

        /// <summary>
        /// Converts a precise input time to the more approximate time that we want to display to the user.
        /// </summary>
        private static TimeSpan GetMappedTime(TimeSpan inputTime, int minuteGranularity, TimeMappingStrategy mappingStrategy)
        {
            // The truncate strategy, in which we round down. Just take the modulo of the minute value and return the result.
            if(mappingStrategy == TimeMappingStrategy.Truncate)
            {
                return new TimeSpan(inputTime.Hours, inputTime.Minutes - inputTime.Minutes % minuteGranularity, 0);
            }
            // The nearest strategy, in which we round to the nearest value according to our desired precision.
            else if(mappingStrategy == TimeMappingStrategy.Nearest)
            {
                if(DisplayTimes == null)
                {
                    DisplayTimes = new HashSet<TimeSpan>();
                    
                    // We want to include "24:00" times for math convenience.
                    for(int hour = 0; hour <= 24; hour++)
                    {
                        for(int minute = 0; minute < 60; minute += minuteGranularity)
                        {
                            // Why use timespan? People who have thought about it way more than I haven't come up with anything better.
                            // https://stackoverflow.com/questions/2037283/how-do-i-represent-a-time-only-value-in-net
                            DisplayTimes.Add(new TimeSpan(hour, minute, 0));
                        }
                    }
                }

                // Map the input time to the closest precomputed valid time.
                TimeSpan mappedTime = DisplayTimes.OrderBy(time => Math.Abs(time.TotalMilliseconds - inputTime.TotalMilliseconds)).First();
                
                // Map 24:00 -> 0:00.
                if(mappedTime.Hours == 24)
                {
                    mappedTime = new TimeSpan(0, mappedTime.Minutes, 0);
                }

                return mappedTime;
            }

            throw new NotImplementedException(String.Format("Don't know how to implement mapping strategy {0}", mappingStrategy.ToString()));
        }
    }
}