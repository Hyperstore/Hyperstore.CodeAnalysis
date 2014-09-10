using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Editor.Completion
{
    class HyperstoreCompletionSet : CompletionSet
    {
        public HyperstoreCompletionSet(string moniker, string displayName, ITrackingSpan applicableTo, IEnumerable<Microsoft.VisualStudio.Language.Intellisense.Completion> completions, IEnumerable<Microsoft.VisualStudio.Language.Intellisense.Completion> completionBuilders)
            : base(moniker, displayName, applicableTo, completions, completionBuilders)
        {
        }

        private CompletionMatchResult MatchCompletionListInternal(IList<Microsoft.VisualStudio.Language.Intellisense.Completion> completionList, CompletionMatchType matchType, bool caseSensitive)
        {
            if (ApplicableTo == null)
            {
                throw new InvalidOperationException("Cannot match completion set with no applicability span.");
            }

            var currentSnapshot = ApplicableTo.TextBuffer.CurrentSnapshot;
            string text = ApplicableTo.GetText(currentSnapshot);
            if (text.Length != 0)
            {
                Microsoft.VisualStudio.Language.Intellisense.Completion bestMatch = null;
                int maxMatchPosition = -1;
                bool isUnique = false;
                bool isSelected = false;
                foreach (var currentCompletion in completionList)
                {
                    string displayText = string.Empty;
                    if (matchType == CompletionMatchType.MatchDisplayText)
                    {
                        displayText = currentCompletion.DisplayText;
                    }
                    else if (matchType == CompletionMatchType.MatchInsertionText)
                    {
                        displayText = currentCompletion.InsertionText;
                    }
                    int matchPositionCount = 0;
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (i >= displayText.Length)
                        {
                            break;
                        }
                        char textChar = text[i];
                        char displayTextChar = displayText[i];
                        if (!caseSensitive)
                        {
                            textChar = char.ToLowerInvariant(textChar);
                            displayTextChar = char.ToLowerInvariant(displayTextChar);
                        }
                        if (textChar != displayTextChar)
                        {
                            break;
                        }
                        matchPositionCount++;
                    }
                    if (matchPositionCount > maxMatchPosition)
                    {
                        maxMatchPosition = matchPositionCount;
                        bestMatch = currentCompletion;
                        isUnique = true;
                        if ((matchPositionCount == text.Length) && (maxMatchPosition > 0))
                        {
                            isSelected = true;
                        }
                    }
                    else if (matchPositionCount == maxMatchPosition)
                    {
                        isUnique = false;
                        if (isSelected)
                        {
                            break;
                        }
                    }
                }
                if (bestMatch != null)
                {
                    CompletionMatchResult result = new CompletionMatchResult();
                    result.SelectionStatus = new CompletionSelectionStatus(bestMatch, isSelected, isUnique);
                    result.CharsMatchedCount = (maxMatchPosition >= 0) ? maxMatchPosition : 0;
                    return result;
                }
            }
            return null;
        }

        public override void SelectBestMatch()
        {
            var matchType = CompletionMatchType.MatchDisplayText;
            var caseSensitive = false;

            CompletionMatchResult matchedCompletions = MatchCompletionListInternal(Completions, matchType, caseSensitive);
            CompletionMatchResult matchedCompletionBuilders = MatchCompletionListInternal(CompletionBuilders, matchType, caseSensitive);
            int completionBuilderCount = 0;
            if (matchedCompletionBuilders != null)
            {
                completionBuilderCount = (matchedCompletionBuilders.CharsMatchedCount + (matchedCompletionBuilders.SelectionStatus.IsSelected ? 1 : 0)) + (matchedCompletionBuilders.SelectionStatus.IsUnique ? 1 : 0);
            }
            int completionCount = 0;
            if (matchedCompletions != null)
            {
                completionCount = (matchedCompletions.CharsMatchedCount + (matchedCompletions.SelectionStatus.IsSelected ? 1 : 0)) + (matchedCompletions.SelectionStatus.IsUnique ? 1 : 0);
            }
            if ((completionBuilderCount > completionCount) && (matchedCompletionBuilders != null))
            {
                SelectionStatus = matchedCompletionBuilders.SelectionStatus;
            }
            else if (matchedCompletions != null)
            {
                SelectionStatus = matchedCompletions.SelectionStatus;
            }
            else if (Completions.Count > 0)
            {
                if (!Completions.Contains(SelectionStatus.Completion))
                {
                    SelectionStatus = new CompletionSelectionStatus(Completions[0], false, false);
                }
            }
            else if (CompletionBuilders.Count > 0)
            {
                if (!CompletionBuilders.Contains(SelectionStatus.Completion))
                {
                    SelectionStatus = new CompletionSelectionStatus(CompletionBuilders[0], false, false);
                }
            }
            else
            {
                SelectionStatus = new CompletionSelectionStatus(null, false, false);
            }
        }


    }
}
