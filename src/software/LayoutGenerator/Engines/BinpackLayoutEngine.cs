using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using JCClock.Common;
using JCClock.Common.Logging;
using JCClock.LayoutGenerator;

namespace JCClock.LayoutGenerator.Engines
{
    public class BinpackLayoutEngine : ILayoutEngine
    {
        private ILogger logger;
        
        public BinpackLayoutEngine(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger(this.GetType().Name);
        }

        public LayoutResult AttemptLayout(int width, int height, IEnumerable<LayoutPhrase> phrases)
        {

/*
while(options remain)
{
	pop option
	if(no room for word)
	{
		abandon option
	}

	insert word at next location
	calculate prime choices
	while(prime choices remain and any can fit)
	{
		if(room in row for any prime choice)
		{
			insert prime choice
			calculate prime choices
		}
		else
		{
			advance to next row
		}
	}

    if(is solution) record.
	put secondary options in queue	
}
/**/


            Stack<LayoutOption> options = new Stack<LayoutOption>();
            List<Layout> solutions = new List<Layout>();
            Layout emptyLayout = Layout.GetEmpty(width, height);
            
            foreach(string firstWord in phrases.Select(phrase => phrase.LayoutWords.First().Text).Distinct())
            {
                options.Push(new LayoutOption(emptyLayout, 0, firstWord));
            }

            while (options.Count != 0)
            {
                logger.Log("Choices left: {0}", options.Count);

                LayoutOption option = options.Pop();
                int currentIndex = option.Index;
                Layout layoutInProgress = option.PartialLayout;
                string nextWord = option.NextWord;
                LayoutEvaluation evaluation;
                IEnumerable<string> primeCandidateWords = null;
                IEnumerable<string> secondaryCandidateWords = null;

                if(!InsertAndEvaluate(layoutInProgress, nextWord, ref currentIndex, phrases, out evaluation, out primeCandidateWords, out secondaryCandidateWords))
                {
                    // Can't insert word; abandon.
                    continue;
                }

                if(primeCandidateWords.Count() == 1 && primeCandidateWords.First() == "OCLOCK")
                {
                    int i = 2;
                    i++;
                }

                // Continue packing while we have obviously good choices AND at least one can fit.
                while(primeCandidateWords.Where(word => !String.IsNullOrEmpty(word) && layoutInProgress.CanFit(word, currentIndex)).Count() != 0)
                {
                    IEnumerable<string> wordsThatCanFitOnCurrentRow = primeCandidateWords.Where(word => !String.IsNullOrEmpty(word) && layoutInProgress.SpaceLeftInRow(currentIndex) >= word.Length);
                    if(wordsThatCanFitOnCurrentRow.Count() != 0)
                    {
                        nextWord = wordsThatCanFitOnCurrentRow.OrderByDescending(word => word.Length).First();
                        if(!InsertAndEvaluate(layoutInProgress, nextWord, ref currentIndex, phrases, out evaluation, out primeCandidateWords, out secondaryCandidateWords))
                        {
                            //TODO better handling.
                            throw new Exception("This should never happen.");
                        }
                    }
                    else
                    {
                        currentIndex = layoutInProgress.GetNextRowStart(currentIndex);

                        // We just bumped to the next row, trim everybody.
                        primeCandidateWords = primeCandidateWords.Where(word => !String.IsNullOrEmpty(word)).Select(word => word.Trim());
                    }

                    if (primeCandidateWords.Count() == 1 && primeCandidateWords.First() == "OCLOCK")
                    {
                        int i = 2;
                        i++;
                    }
                }

                if (evaluation.Quality == evaluation.TargetQuality)
                {
                    logger.Log("Solution found!");
                    layoutInProgress.Print(logger);
                    solutions.Add(layoutInProgress);
                    throw new Exception("Holy shit it works!");
                }

                // We need some way of ranking the goodness of the squishy options. We have to trim the evaluation space.
                // On one execution I saw it enter OCLOCK fifty times consecutively.
                // 1. What is the remaining cardinality of each word?
                // 2. For each word that's not next in line, how close is it to next in line on any phrase?

                // There must always be some word that's ready to go next. Only those should get queued here.
                // We need to build out some idea of what's the score or readiness of each word. A word that's
                // ready in one sentence but not ready in other sentences should have its score pulled down
                // according to the number of times it may have to be revisited.

                // WE SHOULD NEVER SELECT A WORD WITH GREATER THAN 0 IMMEDIACY SINCE WE KNOW IT WON'T HELP ANY OF THE
                // PHRASES THAT CONTRIBUTED IT.

                // But we're already doing that b/c of the way we generate the list. Rely on cardinality, not immediacy.

                // Maybe abandon the stack entirely? 

                logger.Log("Prime words exhausted.");
                int maxRemainingCardinality = evaluation.RemainingCardinality.Select(word => word.Cardinality).Max();
                logger.Log("Max cardinality remaining: {0}", maxRemainingCardinality);
                string[] layoutWordsWithMaxCardinality = evaluation.RemainingCardinality.Where(word => word.Cardinality == maxRemainingCardinality).Select(word => word.Text).ToArray();

                // preserve trimming
                string[] bestOptions = secondaryCandidateWords.Where(word => layoutWordsWithMaxCardinality.Contains(word.Trim())).ToArray();

                logger.Log("Queueing best options: " + String.Join(" ", bestOptions));
                foreach(string word in bestOptions)
                {
                    options.Push(new LayoutOption(Layout.Parse(layoutInProgress.ToString()), currentIndex, word));
                }


                /*

                if (evaluation.Quality == evaluation.TargetQuality)
                {
                    logger.Log("Solution found!");
                    layoutInProgress.Print(logger);
                    solutions.Add(layoutInProgress);
                }


                
                while (primeCandidateWords.Count() != 0)
                {
                    int spaceLeft = layoutInProgress.SpaceLeftInRow(currentIndex);
                    IEnumerable<string> wordsThatFitOnRow = primeCandidateWords.Where(word => word.Length <= spaceLeft);
                    if(wordsThatFitOnRow.Count() != 0)
                    {
                        string newWord = wordsThatFitOnRow.OrderByDescending(word => word.Length).First();

                        logger.Log("Adding {0} at position {1}.", newWord, currentIndex);
                        layoutInProgress.InsertWord(newWord, currentIndex);
                        currentIndex += newWord.Length;

                        evaluation = Layout.Evaluate(layoutInProgress, phrases, logger);
                        LogEvaluation(evaluation);

                        if (evaluation.Quality == evaluation.TargetQuality)
                        {
                            //TODO terminate?
                            logger.Log("Solution found!");
                            layoutInProgress.Print(logger);
                            solutions.Add(layoutInProgress);
                        }

                        justMatched = evaluation.Matches.Where(match => match.LastMatchedWordEnd == currentIndex - 1);
                        justMatchedNextWords = justMatched.Select(match => match.NextWordToMatch).Distinct();
                        primeCandidateWords = evaluation.PrimeCandidateWords.Select(word => justMatchedNextWords.Contains(word) ? " " + word : word);
                    }
                    else
                    {
                        currentIndex = layoutInProgress.GetNextRowStart(currentIndex);
                        if(currentIndex == -1)
                        {
                            logger.Log("Space exhausted, abandoning.");
                            break;
                        }
                    }
                }

                if (currentIndex != -1)
                {
                    // NEED DEEP COPY.
                    // pick high cardinality first.
                    secondaryCandidateWords = evaluation.SecondaryCandidateWords.Select(word => justMatchedNextWords.Contains(word) ? " " + word : word);
                    foreach (string word in secondaryCandidateWords)
                    {
                        options.Push(new LayoutOption(layoutInProgress, currentIndex, word));
                    }
                }


                */
            }

            if(solutions.Count == 0)
            {
                logger.Log("No solutions found.");
            }

            return new LayoutResult();
        }

        private bool InsertAndEvaluate(Layout layout, string word, ref int index, IEnumerable<LayoutPhrase> phrases, out LayoutEvaluation evaluation, out IEnumerable<string> paddedPrimeWords, out IEnumerable<string> paddedSecondaryWords)
        {
            paddedPrimeWords = null;
            paddedSecondaryWords = null;
            evaluation = null;

            if(!layout.CanFit(word, index))
            {
                logger.Log("Cannot fit {0} at or after position {1}.", word, index);
                return false;
            }

            if (layout.SpaceLeftInRow(index) < word.Length)
            {
                index = layout.GetNextRowStart(index);
                
                // No need to have extra spaces since there's no word before us now.
                word = word.Trim();
            }

            logger.Log("Adding {0} at position {1}.", word, index);
            layout.InsertWord(word, index);
            index += word.Length;
            
            evaluation = Layout.Evaluate(layout, phrases, logger);
            LogEvaluation(evaluation);

            int currentIndex = index;
            IEnumerable<PhraseMatch> justMatched = evaluation.Matches.Where(match => match.LastMatchedWordEnd == currentIndex - 1);
            IEnumerable<string> justMatchedNextWords = justMatched.Select(match => match.NextWordToMatch).Distinct();
            paddedPrimeWords = evaluation.PrimeCandidateWords.Select(word => justMatchedNextWords.Contains(word) ? " " + word : word);
            paddedSecondaryWords = evaluation.SecondaryCandidateWords.Select(word => justMatchedNextWords.Contains(word) ? " " + word : word);

            return true;
        }

        private void LogEvaluation(LayoutEvaluation evaluation)
        {
            logger.Log("Total match quality:  {0:0.000}", evaluation.Quality);
            logger.Log("Target match quality: {0:0.000}", evaluation.TargetQuality);
            logger.Log("Best candidate words for addition: {0}", String.Join(' ', evaluation.PrimeCandidateWords));
            logger.Log("Secondary candidate words for addition: {0}", String.Join(' ', evaluation.SecondaryCandidateWords));
        }
    }
}
