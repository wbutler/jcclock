using CommandLine;

using JCClock.Common;

namespace JCClock.LayoutGenerator
{
    /// <summary>
    /// Implements parsing of command-line arguments.
    /// </summary>
    public class LayoutGeneratorArguments
    {
        [Option('i', "inputPath", Required = false, HelpText = "Path to the input file of required phrases.", Default = DefaultValues.InputPath)]
        public string InputPath
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Default values for arguments that are not specified.
        /// </summary>
        public class DefaultValues
        {
            public const string InputPath = @".\phrases.json";
        }

        /// <summary>
        /// Parses a raw argument vector from the command line and returns strongly typed config values.
        /// </summary>
        public static LayoutGeneratorArguments Parse(string[] args)
        {
            return CommandLine.Parser.Default.ParseArguments<LayoutGeneratorArguments>(args)
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
