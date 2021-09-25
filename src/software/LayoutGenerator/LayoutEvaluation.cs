using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCClock.LayoutGenerator
{
    public class LayoutEvaluation
    {
        public double Quality
        {
            get;
            private set;
        }

        public double TargetQuality
        {
            get;
            private set;
        }

        public IEnumerable<PhraseMatch> Matches
        {
            get;
            private set;
        }

        public IEnumerable<string> PrimeCandidateWords
        {
            get;
            private set;
        }

        public IEnumerable<string> SecondaryCandidateWords
        {
            get;
            private set;
        }

        public IEnumerable<LayoutPhraseWord> RemainingCardinality
        {
            get;
            private set;
        }

        public LayoutEvaluation
            (
            double quality,
            double targetQuality,
            IEnumerable<PhraseMatch> matches,
            IEnumerable<string> primeCandidateWords,
            IEnumerable<string> secondaryCandidateWords,
            IEnumerable<LayoutPhraseWord> remainingCardinality
            )
        {
            Quality = quality;
            TargetQuality = targetQuality;
            Matches = matches;
            PrimeCandidateWords = primeCandidateWords;
            SecondaryCandidateWords = secondaryCandidateWords;
            RemainingCardinality = remainingCardinality;
        }
    }
}
