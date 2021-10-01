﻿using System;
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
        private Dictionary<string, string> layoutGenealogy;

        private bool showGenealogy;

        public BinpackLayoutEngine(bool showGenealogy)
        {
            this.showGenealogy = showGenealogy;
        }

        public IEnumerable<Layout> AttemptLayout(int width, int height, IEnumerable<LayoutPhrase> phrases, ILogger logger)
        {
            layoutGenealogy = new Dictionary<string, string>();
            Stack<LayoutOption> options = new Stack<LayoutOption>();
            List<Layout> solutions = new List<Layout>();
            Layout emptyLayout = Layout.GetEmpty(width, height);
            
            foreach(string firstWord in phrases.Select(phrase => phrase.LayoutWords.First().Text).Distinct())
            {
                options.Push(new LayoutOption(emptyLayout.ToString(), 0, firstWord));
            }

            while (options.Count != 0)
            {
                logger?.Log("Choices left: {0}", options.Count);

                LayoutOption option = options.Pop();
                int currentIndex = option.Index;
                Layout layoutInProgress = option.PartialLayout;
                string nextWord = option.NextWord;
                LayoutEvaluation evaluation;
                IEnumerable<string> primeCandidateWords = null;
                IEnumerable<string> secondaryCandidateWords = null;

                string originalLayout = layoutInProgress.ToString();
                if (!InsertAndEvaluate(layoutInProgress, nextWord, ref currentIndex, phrases, out evaluation, out primeCandidateWords, out secondaryCandidateWords, logger))
                {
                    // Can't insert word; abandon.
                    continue;
                }
                //FIXFIXFIX put in InsertAndEvaluate
                layoutGenealogy[layoutInProgress.ToString()] = originalLayout;


                // Continue packing while we have obviously good choices AND at least one can fit.
                while (primeCandidateWords.Where(word => !String.IsNullOrEmpty(word) && layoutInProgress.CanFit(word, currentIndex)).Count() != 0)
                {
                    IEnumerable<string> wordsThatCanFitOnCurrentRow = primeCandidateWords.Where(word => !String.IsNullOrEmpty(word) && layoutInProgress.SpaceLeftInRow(currentIndex) >= word.Length);
                    if(wordsThatCanFitOnCurrentRow.Count() != 0)
                    {
                        nextWord = wordsThatCanFitOnCurrentRow.OrderByDescending(word => word.Length).First();
                        originalLayout = layoutInProgress.ToString();
                        if(!InsertAndEvaluate(layoutInProgress, nextWord, ref currentIndex, phrases, out evaluation, out primeCandidateWords, out secondaryCandidateWords, logger))
                        {
                            //TODO better handling.
                            throw new Exception("This should never happen.");
                        }
                        layoutGenealogy[layoutInProgress.ToString()] = originalLayout;
                    }
                    else
                    {
                        currentIndex = layoutInProgress.GetNextRowStart(currentIndex);

                        // We just bumped to the next row, trim everybody.
                        primeCandidateWords = primeCandidateWords.Where(word => !String.IsNullOrEmpty(word)).Select(word => word.Trim());
                    }
                }

                //
                //
                // NEVER INSERT EMPTY SPACES ALONE
                //
                //

                if (primeCandidateWords.Count() == 0)
                {
                    logger?.Log("Prime words exhausted.");
                }
                
                int spaceLeft = layoutInProgress.SpaceAfterIndex(currentIndex);
                int remainingChars = evaluation.RemainingCharacters;
                if(remainingChars > spaceLeft)
                {
                    logger?.Log("{0} chars remaining for placement, but {1} positions left in layout. Abandoning.", remainingChars, spaceLeft);
                    continue;
                }                

                if (evaluation.Quality == evaluation.TargetQuality)
                {
                    logger?.Log("Solution found!");
                    layoutInProgress.Print(logger);
                    logger?.Log("");
                    solutions.Add(layoutInProgress);
                }
                else
                {
                    int maxRemainingCardinality = evaluation.RemainingCardinality.Select(word => word.Cardinality).Max();
                    logger?.Log("Max cardinality remaining: {0}", maxRemainingCardinality);
                    string[] layoutWordsWithMaxCardinality = evaluation.RemainingCardinality.Where(word => word.Cardinality == maxRemainingCardinality).Select(word => word.Text).ToArray();

                    // preserve trimming
                    string[] bestOptions = secondaryCandidateWords.Where(word => layoutWordsWithMaxCardinality.Contains(word.Trim())).ToArray();

                    logger?.Log("Queueing best options: " + String.Join(" ", bestOptions));
                    foreach (string word in bestOptions)
                    {
                        options.Push(new LayoutOption(layoutInProgress.ToString(), currentIndex, word));
                    }
                }
            }

            if(solutions.Count != 0)
            {
                if (showGenealogy)
                {
                    logger?.Log("\nBEGIN SOLUTION GENEALOGY\n");

                    int i = 0;
                    foreach (Layout solutionLayout in solutions)
                    {
                        logger?.Log("\nSOLUTION {0}\n", i);
                        PrintSolutionGenealogy(solutionLayout, logger);
                        i++;
                    }
                }
            }
            else
            {
                logger?.Log("No solutions found.");
            }

            return solutions;
        }

        private void PrintSolutionGenealogy(Layout layout, ILogger logger)
        {
            if (logger != null)
            {
                int generation = 0;
                while (!layout.IsEmpty())
                {
                    logger.Log("Generation {0}", generation);
                    layout.Print(logger);
                    logger.Log("");
                    generation++;
                    layout = Layout.Parse(layoutGenealogy[layout.ToString()]);
                }
            }
        }

        private bool InsertAndEvaluate(Layout layout, string word, ref int index, IEnumerable<LayoutPhrase> phrases, out LayoutEvaluation evaluation, out IEnumerable<string> paddedPrimeWords, out IEnumerable<string> paddedSecondaryWords, ILogger logger = null)
        {
            paddedPrimeWords = null;
            paddedSecondaryWords = null;
            evaluation = null;

            if(!layout.CanFit(word, index))
            {
                logger?.Log("Cannot fit {0} at or after position {1}.", word, index);
                return false;
            }

            if (layout.SpaceLeftInRow(index) < word.Length)
            {
                index = layout.GetNextRowStart(index);
                
                // No need to have extra spaces since there's no word before us now.
                word = word.Trim();
            }

            logger?.Log("Adding {0} at position {1}.", word, index);
            layout.InsertWord(word, index);
            index += word.Length;
            
            evaluation = Layout.Evaluate(layout, phrases, logger);
            LogEvaluation(evaluation, logger);

            int currentIndex = index;
            IEnumerable<PhraseMatch> justMatched = evaluation.Matches.Where(match => match.LastMatchedWordEnd == currentIndex - 1);
            IEnumerable<string> justMatchedNextWords = justMatched.Select(match => match.NextWordToMatch).Distinct();
            paddedPrimeWords = evaluation.PrimeCandidateWords.Select(word => justMatchedNextWords.Contains(word) ? " " + word : word);
            paddedSecondaryWords = evaluation.SecondaryCandidateWords.Select(word => justMatchedNextWords.Contains(word) ? " " + word : word);

            return true;
        }

        private void LogEvaluation(LayoutEvaluation evaluation, ILogger logger = null)
        {
            logger?.Log("Total match quality:  {0:0.000}", evaluation.Quality);
            logger?.Log("Target match quality: {0:0.000}", evaluation.TargetQuality);
            logger?.Log("Best candidate words for addition: {0}", String.Join(' ', evaluation.PrimeCandidateWords));
            logger?.Log("Secondary candidate words for addition: {0}", String.Join(' ', evaluation.SecondaryCandidateWords));
        }
    }
}
