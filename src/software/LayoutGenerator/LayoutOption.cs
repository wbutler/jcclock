namespace JCClock.LayoutGenerator
{
    public class LayoutOption
    {
        public Layout PartialLayout
        {
            get;
            private set;
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

        public LayoutOption(Layout partialLayout, int index, string nextWord)
        {
            PartialLayout = partialLayout;
            Index = index;
            NextWord = nextWord;
        }
    }
}
