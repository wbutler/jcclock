using System;

namespace JCClock.PhraseGenerator
{
    /// <summary>
    /// Utility class that translates an integer to a natural language string.
    /// </summary>
    public class NumberTextTranslator
    {
        /// <summary>
        /// We only translate integers less than this value.
        /// </summary>
        private static readonly int maxValue = 60;

        /// <summary>
        /// Singleton class instance. Callers should use the static accessor.
        /// </summary>
        private static NumberTextTranslator instance;

        /// <summary>
        /// Synchronizes access to the instance and its initialization.
        /// </summary>
        private static object initLock = new object();

        /// <summary>
        /// Developer-provided input to represent the key values from which other strings are constructed.
        /// </summary>
        private string[] words = new string[maxValue];

        /// <summary>
        /// Cached, finished output strings.
        /// </summary>
        private string[] translations = new string[maxValue];

        private NumberTextTranslator()
        {
            words[0] = "";
            words[1] = "one";
            words[2] = "two";
            words[3] = "three";
            words[4] = "four";
            words[5] = "five";
            words[6] = "six";
            words[7] = "seven";
            words[8] = "eight";
            words[9] = "nine";
            words[10] = "ten";
            words[11] = "eleven";
            words[12] = "twelve";
            words[13] = "thirteen";
            words[14] = "fourteen";
            words[15] = "fifteen";
            words[16] = "sixteen";
            words[17] = "seventeen";
            words[18] = "eighteen";
            words[19] = "nineteen";
            words[20] = "twenty";
            words[30] = "thirty";
            words[40] = "forty";
            words[50] = "fifty";

            // These values won't change and the domain is small;
            // just precompute everything so it's fast.
            for(int i = 0; i < maxValue; i++)
            {
                translations[i] = Translate(i);
            }
        }

        /// <summary>
        /// Translate a given integer into its textual representation.
        /// </summary>
        public static string Get(int number)
        {
            lock (initLock)
            {
                // In this application, we can tolerate the first call taking a while.
                if(instance == null)
                {
                    instance = new NumberTextTranslator();
                }
            }

            if(number < 0 || number > maxValue)
            {
                throw new ArgumentException(String.Format("Supports only 0 < n < {0}", maxValue));
            }

            return instance.translations[number];
        }

        /// <summary>
        /// Internal helper; actually do the uncached work of building a text string for a given number.
        /// </summary>
        /// <returns></returns>
        private string Translate(int number)
        {
            // Zeros are a special case here; we just don't show anything.
            if (number == 0)
            {
                return "";
            }

            // In English, almost any value < 20 is a special or base case. Easier to just explicitly specify.
            if (number < 20)
            {
                return words[number];
            }

            // General case for values >= 20.
            if (number < maxValue)
            {
                string tens = words[(number / 10) * 10];
                string units = words[number % 10];

                return String.Format("{0} {1}", tens, units);
            }

            throw new ArgumentException(String.Format("Cannot translate value {0}.", number));
        }
    }
}
