using System;
using System.Collections.Generic;
using System.Linq;

namespace JCClock.LayoutGenerator
{
    /// <summary>
    /// Data container class that represents the result of attempting to display a given phrase on a given layout.
    /// </summary>
    public class PhraseMatch
    {
        /// <summary>
        /// The quality of the match. If a given layout can fully display a phrase, this value is 1. If not,
        /// this value is equal to the fraction of characters in the phrase that the layout can properly display.
        /// </summary>
        public double Quality
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether or not the layout can fully display the given phrase.
        /// </summary>
        public bool IsFullMatch
        {
            get;
            private set;
        }

        public string NextWordToMatch
        {
            get;
            private set;
        }

        public IEnumerable<string> UnmatchedWords
        {
            get;
            private set;
        }

        public IEnumerable<string> FutureUnmatchedWords
        {
            get;
            private set;
        }

        /// <summary>
        /// Array of objects mapping each word in the phrase to its location on the layout.
        /// </summary>
        public WordMatch[] WordMatches
        {
            get;
            private set;
        }

        public int LastMatchedWordEnd
        {
            get;
            private set;
        }

        public IEnumerable<LayoutPhraseWord> RemainingCardinality
        {
            get;
            private set;
        }

        public PhraseMatch(LayoutPhrase phrase, WordMatch[] wordMatches)
        {
            int totalChars = phrase.Characters;
            int matchChars = 0;

            LastMatchedWordEnd = Int32.MinValue;
            IsFullMatch = true;

            Dictionary<string, int> remainingCardinality = new Dictionary<string, int>();
            foreach (LayoutPhraseWord word in phrase.LayoutWords)
            {
                if (!remainingCardinality.ContainsKey(word.Text))
                {
                    remainingCardinality[word.Text] = 0;
                }
                remainingCardinality[word.Text] += word.Cardinality;
            }

            foreach (WordMatch match in wordMatches)
            {
                if (match != null)
                {
                    matchChars += match.Length;
                }

                if (match == null || !match.IsFullMatch)
                {
                    IsFullMatch = false;
                }
                else // This is a full match.
                {
                    // The index where our good matches end.
                    LastMatchedWordEnd = match.LocationIndex + match.Length - 1;
                    remainingCardinality[match.WordText]--;
                }
            }

            if (!IsFullMatch)
            {
                List<string> unmatchedWords = new List<string>();
                for (int i = 0; i < phrase.LayoutWords.Length; i++)
                {
                    if (wordMatches[i] == null || !wordMatches[i].IsFullMatch)
                    {
                        if (NextWordToMatch == null)
                        {
                            NextWordToMatch = phrase.LayoutWords[i].Text;
                        }
                        unmatchedWords.Add(phrase.LayoutWords[i].Text);
                    }
                }

                UnmatchedWords = new List<string>(unmatchedWords);
                List<string> futureUnmatchedWords = UnmatchedWords.Skip(1).ToList();
                FutureUnmatchedWords = futureUnmatchedWords;
            }

            Quality = (double)matchChars / (double)totalChars;
            WordMatches = wordMatches;
            RemainingCardinality = remainingCardinality.Keys.Select(word => new LayoutPhraseWord(word, remainingCardinality[word])).ToList();
        }
    }
}
