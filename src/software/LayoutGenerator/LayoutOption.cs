namespace JCClock.LayoutGenerator
{
    public class LayoutOption
    {
        public Layout PartialLayout
        {
            get
            {
                return Layout.Parse(layoutBuffer);
            }
        }

        public int Index
        {
            get;
            private set;
        }

        public string NextWord
        {
            get;
            private set;
        }

        private string layoutBuffer;

        public LayoutOption(string layout, int index, string nextWord)
        {
            layoutBuffer = layout;
            Index = index;
            NextWord = nextWord;
        }
    }
}
