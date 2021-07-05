namespace JCClock.Common
{
    /// <summary>
    /// Data container class that maps a time to a human-readable display string.
    /// </summary>
    public class TimeDisplayMapping
    {
        /// <summary>
        /// The integer hour, 0-23.
        /// </summary>
        public int Hour { get; set; }

        /// <summary>
        /// The integer minute, 0-59.
        /// </summary>
        public int Minute { get; set; }

        /// <summary>
        /// The text to output at the corresponding time <hour>:<minute>.
        /// </summary>
        public string DisplayText { get; set; }

        public TimeDisplayMapping()
        { }

        public TimeDisplayMapping(int hour, int minute, string displayText)
        {
            Hour = hour;
            Minute = minute;
            DisplayText = displayText;
        }
    }
}
