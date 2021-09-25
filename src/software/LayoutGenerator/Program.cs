using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using JCClock.Common;
using JCClock.Common.Logging;
using JCClock.LayoutGenerator.Engines;

namespace JCClock.LayoutGenerator
{
    class Program
    {
        public static void Main(string[] args)
        {
            using (ILoggerFactory loggerFactory = ConsoleLoggerFactory.Get())
            {
                ILogger logger = loggerFactory.CreateLogger("LayoutGenerator");
                try
                {
                    // Parse our command line args.
                    LayoutGeneratorArguments config = LayoutGeneratorArguments.Parse(args);
                    string inputPath = config.InputPath;

                    // Read in and parse the file of required phrases.
                    ClockPhraseCollection phraseCollection = JsonConvert.DeserializeObject<ClockPhraseCollection>(File.ReadAllText(inputPath));
                    logger.Log("Loaded {0}", inputPath);

                    // The input file contains mappings from every HH:MM time to the phrase that represents it.
                    // There will be duplication. Dedup the phrases before generating the layout.
                    HashSet<string> inputPhrases = new HashSet<string>();
                    foreach (TimeDisplayMapping mapping in phraseCollection.TimePhrases)
                    {
                        if (!inputPhrases.Contains(mapping.DisplayText))
                        {
                            inputPhrases.Add(mapping.DisplayText);
                        }
                    }
                    foreach(string specialPhrase in phraseCollection.SpecialPhrases)
                    {
                        inputPhrases.Add(specialPhrase);
                    }
                    logger.Log("{0} phrases in input.", inputPhrases.Count);

                    // Parse the input phrases into a friendlier format for later processing.
                    List<LayoutPhrase> phrases = new List<LayoutPhrase>();
                    foreach (string phrase in inputPhrases)
                    {
                        phrases.Add(LayoutPhrase.Parse(phrase));
                    }

                    (int width, int height) = FindMinimumFrameSize(phrases, logger);
                    int longestWordLength = phrases.SelectMany(phrase => phrase.Words).Select(word => word.Length).Max();
                    if(longestWordLength > width)
                    {
                        logger.Log("Longest word in input has {0} characters. Setting this value as minimum frame width.", longestWordLength);
                        width = longestWordLength;
                    }

                    ILayoutEngine layoutEngine = new BinpackLayoutEngine();
                    IEnumerable<Layout> layouts = new List<Layout>();
                    while (layouts.Count() == 0)
                    {
                        double aspectRatio = (double)width / (double)height;
                        logger.Log("Attempting layout {0} wide by {1} high with {2} characters.", width, height, width * height);
                        logger.Log("Frame has aspect ratio {0:0.00} vs target {1:0.00}", aspectRatio, Constants.PreferredWidthHeightRatio);

                        layouts = layoutEngine.AttemptLayout(width, height, phrases, logger);
                        logger.Log("Found {0} valid layouts at this size.", layouts.Count());
                        if (layouts.Count() > 0)
                        {
                            foreach (Layout layout in layouts)
                            {
                                layout.Print(logger);
                                logger.Log("");
                            }
                        }
                        else
                        {
                            if(aspectRatio <= Constants.PreferredWidthHeightRatio)
                            {
                                width++;
                            }
                            else
                            {
                                height++;
                            }
                        }
                    }
                }

                // We hit this and do nothing (exit cleanly) if we failed to parse our command-line args.
                catch (CommandLineParseException)
                { }

                catch (Exception ex)
                {
                    logger.LogError(ex, "Global Exception.");
                }
            }
        }

        private static (int, int) FindMinimumFrameSize(IEnumerable<LayoutPhrase> phrases, ILogger logger = null)
        {
            int width, height;

            Dictionary<string, int> wordCardinalities = new Dictionary<string, int>();
            foreach (LayoutPhrase phrase in phrases)
            {
                foreach (LayoutPhraseWord word in phrase.LayoutWords)
                {
                    if (!wordCardinalities.ContainsKey(word.Text))
                    {
                        wordCardinalities[word.Text] = 0;
                    }
                    wordCardinalities[word.Text] = Math.Max(wordCardinalities[word.Text], word.Cardinality);
                }
            }

            logger?.Log("Phrases contain {0} distinct words, {1} with cardinality > 1.",
                wordCardinalities.Keys.Count, wordCardinalities.Values.Where(value => value > 1).Count());
            int minimumChars = wordCardinalities.Keys.Sum(word => word.Length * wordCardinalities[word]);
            logger?.Log("Solution contains no fewer than {0} characters.", minimumChars);

            foreach (string word in wordCardinalities.Keys)
            {
                logger?.Log("{0}, {1}", word, wordCardinalities[word]);
            }

            height = Convert.ToInt32(Math.Floor(Math.Sqrt(minimumChars / Constants.PreferredWidthHeightRatio)));
            width = Convert.ToInt32(Math.Floor(height * Constants.PreferredWidthHeightRatio));
            while (height * width < minimumChars)
            {
                if ((double)width / (double)height < Constants.PreferredWidthHeightRatio)
                {
                    width++;
                }
                else
                {
                    height++;
                }
            }

            return (width, height);
        }
    }
}