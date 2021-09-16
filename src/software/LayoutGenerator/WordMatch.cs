namespace JCClock.LayoutGenerator
{
    /// <summary>
    /// Represents the match location of a single word on a clock layout.
    /// </summary>
    public class WordMatch
    {
        /// <summary>
        /// Index into the layout buffer of the beginning of the word.
        /// </summary>
        public int LocationIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// Number of characters after the first one that the match continues.
        /// </summary>
        public int Length
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether or not the word is fully matched.
        /// </summary>
        public bool IsFullMatch
        {
            get;
            private set;
        }

        /// <summary>
        /// Human-readable text of the word to be matched.
        /// </summary>
        public string WordText
        {
            get;
            private set;
        }

        public WordMatch(int locationIndex, int length, bool isFullMatch, string wordText)
        {
            LocationIndex = locationIndex;
            Length = length;
            IsFullMatch = isFullMatch;
            WordText = wordText;
        }
    }
}
