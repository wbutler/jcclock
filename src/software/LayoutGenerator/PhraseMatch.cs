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

        /// <summary>
        /// Array of objects mapping each word in the phrase to its location on the layout.
        /// </summary>
        public WordMatch[] WordMatches
        {
            get;
            private set;
        }

        public PhraseMatch(LayoutPhrase phrase, WordMatch[] wordMatches)
        {
            int totalChars = phrase.Characters;
            int matchChars = 0;

            IsFullMatch = true;

            foreach (WordMatch match in wordMatches)
            {
                if (match != null)
                {
                    matchChars += match.Length;
                }

                // If any word in the phrase isn't completely matched, then the
                // phrase itself is not completely matched.
                if (match == null || !match.IsFullMatch)
                {
                    IsFullMatch = false;
                }
            }

            Quality = (double)matchChars / (double)totalChars;
            WordMatches = wordMatches;
        }
    }
}
