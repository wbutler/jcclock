using System;

namespace JCClock.LayoutGenerator
{
    /// <summary>
    /// Represents a distinct word that occurs in a phrase.
    /// </summary>
    public class LayoutPhraseWord
    {
        /// <summary>
        /// The human-readable text of the word.
        /// </summary>
        public string Text
        {
            get;
            private set;
        }

        /// <summary>
        /// The count of times that this word appears in its parent phrase.
        /// </summary>
        public int Cardinality
        {
            get;
            private set;
        }

        public LayoutPhraseWord(string text, int cardinality)
        {
            Text = text;
            Cardinality = cardinality;
        }

        /// <summary>
        /// Generate a human-readable debugging string.
        /// </summary>
        public override string ToString()
        {
            return String.Format("{\"{0}\", {1}}", Text, Cardinality);
        }
    }
}
