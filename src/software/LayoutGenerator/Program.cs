using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using JCClock.Common;
using JCClock.Common.Logging;

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

                    // Generate a test layout to exercise the evaluation function.
                    Layout testLayout = Layout.Parse("ITAISAAA|FIVEAAAA|AAATENAA|AAAAAAAA");
                    testLayout.Print(logger);

                    // For each of the input phrases, show how it or the best possible fraction of it
                    // would be rendered on our test layout.
                    foreach(LayoutPhrase phrase in phrases)
                    {
                        PhraseMatch phraseMatch = testLayout.Evaluate(phrase);
                        logger.Log("");
                        logger.Log("Best match for {0}:", phrase.Text);
                        logger.Log("Quality: {0:0.000}", phraseMatch.Quality);
                        testLayout.PrintMatches(phraseMatch.WordMatches, logger);
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
    }
}