using System;
using System.Collections.Generic;

namespace JCClock.LayoutGenerator
{
    /// <summary>
    /// Represents a phrase that a given layout should be able to display.
    /// </summary>
    public class LayoutPhrase
    {
        // The human-readable text of the layout.
        public string Text
        {
            get;
            private set;
        }

        // The words in the phrase, expressed as an array of strings. 
        public string[] Words
        {
            get;
            private set;
        }

        /// <summary>
        /// An array of objects representing each of the distinct words in the phrase
        /// and their associated cardinality.
        /// </summary>
        public LayoutPhraseWord[] LayoutWords
        {
            get;
            private set;
        }

        /// <summary>
        /// The character count of the phrase.
        /// </summary>
        public int Characters
        {
            get
            {
                int sum = 0;
                foreach(string word in Words)
                {
                    sum += word.Length;
                }
                return sum;
            }
        }

        /// <summary>
        /// Instantiates a new instance of LayoutPhrase by parsing a human-readable input string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static LayoutPhrase Parse(string text)
        {
            text = text.ToUpper();
            string[] words = text.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, int> wordCounts = new Dictionary<string, int>();

            foreach (string word in words)
            {
                if (wordCounts.ContainsKey(word))
                {
                    wordCounts[word]++;
                }
                else
                {
                    wordCounts[word] = 1;
                }
            }

            List<LayoutPhraseWord> layoutWords = new List<LayoutPhraseWord>();
            foreach (string word in wordCounts.Keys)
            {
                layoutWords.Add(new LayoutPhraseWord(word, wordCounts[word]));
            }

            return new LayoutPhrase(text, words, layoutWords.ToArray());
        }
        
        private LayoutPhrase(string text, string[] words, LayoutPhraseWord[] layoutWords)
        {
            Text = text;
            Words = words;
            LayoutWords = layoutWords;
        }

    }
}
