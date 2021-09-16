using System;
using System.Text;

using Microsoft.Extensions.Logging;

using JCClock.Common.Logging;

namespace JCClock.LayoutGenerator
{
    /// <summary>
    /// Represents a grid of characters that form the display element of a word clock.
    /// This class is the fundamental output of layout generation.
    /// </summary>
    public class Layout
    {
        protected static class LayoutConstants
        {
            public static readonly char RowDelimiter = '|';
            public static readonly char EmptyChar = '.';
        }

        /// <summary>
        /// The count of rows in the layout.
        /// </summary>
        public int Rows
        {
            get;
            private set;
        }

        /// <summary>
        /// The count of columns in the layout.
        /// </summary>
        public int Columns
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Internal storage for the actual arrangement of characters.
        /// </summary>
        private string LayoutBuffer;

        /// <summary>
        /// Generates a new instance from a string representation.
        /// </summary>
        public static Layout Parse(string source)
        {
            string[] elements = source.Split(LayoutConstants.RowDelimiter);
            for(int i = 0; i < elements.Length - 1; i++)
            {
                if(elements[i].Length != elements[i+1].Length)
                {
                    throw new ArgumentException(
                        String.Format(
                            "Row length mismatch between {0} with length {1} and {2} with length {3}.",
                            elements[i], elements[i].Length, elements[i + 1], elements[i + 1].Length));
                }
            }

            return new Layout(source, elements.Length, elements[0].Length);
        }

        /// <summary>
        /// Writes a pretty-print version of the layout to the provided ILogger.
        /// </summary>
        public void Print(ILogger logger)
        {
            PrintLayoutBuffer(LayoutBuffer, logger);
        }

        /// <summary>
        /// Displays how a given set of matches would appear on the final layout.
        /// </summary>
        public void PrintMatches(WordMatch[] matches, ILogger logger)
        {
            bool[] charMatches = new bool[LayoutBuffer.Length];
            StringBuilder renderedBuffer = new StringBuilder();
            foreach(WordMatch match in matches)
            {
                if (match != null)
                {
                    for (int i = 0; i < match.Length; i++)
                    {
                        charMatches[match.LocationIndex + i] = true;
                    }
                }
            }

            for(int i = 0; i < LayoutBuffer.Length; i++)
            {
                if(charMatches[i] || LayoutBuffer[i] == LayoutConstants.RowDelimiter)
                {
                    renderedBuffer.Append(LayoutBuffer[i]);
                }
                else
                {
                    renderedBuffer.Append('.');
                }
            }

            PrintLayoutBuffer(renderedBuffer.ToString(), logger);
        }

        /// <summary>
        /// Accepts an input phrase and evaluates what fraction of it the current
        /// layout can properly display.
        /// </summary>
        public PhraseMatch Evaluate(LayoutPhrase phrase)
        {
            int wordIndex = 0;
            int layoutIndex = 0;
            WordMatch[] matches = new WordMatch[phrase.Words.Length];

            while(layoutIndex < LayoutBuffer.Length && wordIndex < phrase.Words.Length)
            {
                int matchLength;
                try
                {
                    matchLength = EvaluateWordAtLocation(phrase.Words[wordIndex], layoutIndex);
                }
                catch(IndexOutOfRangeException)
                {
                    throw;
                }

                // If the match as this location is better than what we had, record it.
                bool isFullMatch = matchLength == phrase.Words[wordIndex].Length;
                if (matches[wordIndex] == null || matches[wordIndex].Length < matchLength)
                {
                    matches[wordIndex] = new WordMatch(layoutIndex, matchLength, isFullMatch, phrase.Words[wordIndex]);
                }

                if(isFullMatch)
                {
                    // If we completely matched the word at this location, search for the next word.
                    wordIndex++;

                    // Advance our search index the length of the word plus a space.
                    layoutIndex += matchLength + 1;
                }
                else
                {
                    // We didn't find a complete match here. Go on to the next character.
                    layoutIndex++;
                }
            }

            return new PhraseMatch(phrase, matches);
        }

        /// <summary>
        /// Return a human-readable string for debugging only, not UI or serialization.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return LayoutBuffer;
        }

        /// <summary>
        /// Helper function to do the pretty print work of rendering a layout buffer
        /// for display to an output channel.
        /// </summary>
        private static void PrintLayoutBuffer(string layoutBuffer, ILogger logger)
        {
            foreach (string row in layoutBuffer.Split(LayoutConstants.RowDelimiter))
            {
                StringBuilder rowBuilder = new StringBuilder();
                foreach (char c in row)
                {
                    rowBuilder.Append(c);
                    rowBuilder.Append(' ');
                }
                logger.Log(rowBuilder.ToString().TrimEnd());
            }
        }

        /// <summary>
        /// Constructor for internal use only. Use Parse() to generate new instances.
        /// </summary>
        private Layout(string source, int rows, int columns)
        {
            LayoutBuffer = source.ToUpper();
            Rows = rows;
            Columns = columns;
        }

        /// <summary>
        /// Determines how many characters of the supplied word might be displayed
        /// at the given location index. 
        /// </summary>
        private int EvaluateWordAtLocation(string word, int locationIndex)
        {
            int index = 0;
            while (index < word.Length && locationIndex + index < LayoutBuffer.Length && word[index] == LayoutBuffer[locationIndex + index])
            {
                index++;
            }

            return index;
        }

    }
}
