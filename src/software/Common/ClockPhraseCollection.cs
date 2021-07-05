using System.Collections.Generic;

namespace JCClock.Common
{
    /// <summary>
    /// Data container class describing the total set of phrases that a clock layout should be able to display,
    /// including mappings from time values to the output phrase.
    /// </summary>
    public class ClockPhraseCollection
    {
        /// <summary>
        /// The collection of mappings from hour\minute -> output phrase.
        /// </summary>
        public List<TimeDisplayMapping> TimePhrases
        {
            get;
            set;
        }

        /// <summary>
        /// An unordered list of additional phrases the clock should output,
        /// e.g. holiday greetings, etc.
        /// </summary>
        public List<string> SpecialPhrases
        {
            get;
            set;
        }

        public ClockPhraseCollection()
        {
            TimePhrases = new List<TimeDisplayMapping>();
            SpecialPhrases = new List<string>();
        }
    }
}
