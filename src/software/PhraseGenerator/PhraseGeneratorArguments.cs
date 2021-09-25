using System.IO;

using CommandLine;

using JCClock.Common;

namespace JCClock.PhraseGenerator
{
    /// <summary>
    /// Describes an algorithm for converting an exact time to an approximate time for display.
    /// </summary>
    public enum TimeMappingStrategy
    {
        Truncate,       // Always round down; 12:58 -> 12:55
        Nearest         // Round to nearest; 12:58 -> 1:00
    }

    /// <summary>
    /// Implements parsing of command-line arguments.
    /// </summary>
    public class PhraseGeneratorArguments
    {
        [Option('s', "mappingStrategy", Required = false, HelpText = "The strategy algorithm for mapping precise times to display times.", Default = DefaultValues.MappingStrategy)]
        public TimeMappingStrategy MappingStrategy
        {
            get;
            private set;
        }

        [Option('g', "minuteGranularity", Required = false, HelpText = "The minute granularity of the output strings to generate.", Default = DefaultValues.MinuteGranularity)]
        public int MinuteGranularity
        {
            get;
            private set;
        }

        [Option('o', "outputPath", Required = false, HelpText = "Path to the desired output file.", Default = DefaultValues.OutputPath)]
        public string OutputPath
        {
            get;
            private set;
        }

        [Option('i', "customPhraseFile", Required = false, HelpText = "Path to a text file of additional desired custom phrases, one per line.")]
        public string CustomPhraseFile
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Default values for arguments that are not specified.
        /// </summary>
        public class DefaultValues
        {
            public const TimeMappingStrategy MappingStrategy = TimeMappingStrategy.Nearest;
            public const int MinuteGranularity = 5;
            public const string OutputPath = "phrases.json";
        }

        /// <summary>
        /// Parses a raw argument vector from the command line and returns strongly typed config values.
        /// </summary>
        public static PhraseGeneratorArguments Parse(string[] args)
        {
            return CommandLine.Parser.Default.ParseArguments<PhraseGeneratorArguments>(args)
                .MapResult(
                (options) =>
                {
                    return options;
                },
                (errors) =>
                {
                    throw new CommandLineParseException();
                });
        }
    }
}
