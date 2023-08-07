using text_compare.Core.Interface;
using text_compare.Models;
using static text_compare.Constants.Constants;
using DiffPlex;
using DiffPlex.DiffBuilder;
using System.Text;

namespace text_compare.Services
{
    public class CompareText : ICompareText
    {
        public TextComparisonResult CompareAndHighlightChanges(string baseText, string modifiedText, List<ChangeInfo> changes)
        {
            var baseWithHighlights = ApplyHighlights(baseText, changes);
            var modifiedWithHighlights = ApplyHighlights(modifiedText, changes);

            return new TextComparisonResult
            {
                BaseTextWithHighlights = baseWithHighlights,
                ModifiedTextWithHighlights = modifiedWithHighlights,
                RemovalCount = 0,
                //lineChanges.Count(c => c.Type == ChangeType.Removal),
                AdditionCount = 0
                //lineChanges.Count(c => c.Type == ChangeType.Addition)
            };
        }

        public List<ChangeInfo> CompareLineByLine(string baseText, string modifiedText)
        {
            var baseLines = baseText.Split(Environment.NewLine);
            var modifiedLines = modifiedText.Split(Environment.NewLine);

            return CompareSequences(baseLines, modifiedLines);
        }

        public List<ChangeInfo> CompareWordByWord(string baseText, string modifiedText)
        {
            var baseWords = baseText.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            var modifiedWords = modifiedText.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            return CompareSequences(baseWords, modifiedWords);
        }

        public TextComparisonResult CompareLetterByLetter(string baseText, string modifiedText)
        {
            var baseLetters = baseText.ToCharArray();
            var modifiedLetters = modifiedText.ToCharArray();
            var res = CompareSequences(baseLetters, modifiedLetters);
            return CompareAndHighlightChanges(baseText, modifiedText, res);
        }

        private List<ChangeInfo> CompareSequences<T>(IEnumerable<T> baseSeq, IEnumerable<T> modifiedSeq)
        {
            var differ = new Differ();
            var inlineBuilder = new InlineDiffBuilder(differ);

            var baseText = string.Join("", baseSeq);
            var modifiedText = string.Join("", modifiedSeq);

            var diffResult = inlineBuilder.BuildDiffModel(baseText, modifiedText);

            var changes = new List<ChangeInfo>();
            int baseIndex = 0;
            int modifiedIndex = 0;

            foreach (var line in diffResult.Lines)
            {
                if (line.Type == DiffPlex.DiffBuilder.Model.ChangeType.Unchanged)
                { 
                    baseIndex += line.Text.Length;
                    modifiedIndex += line.Text.Length;
                }
                else if (line.Type == DiffPlex.DiffBuilder.Model.ChangeType.Deleted)
                {
                    changes.Add(new ChangeInfo
                    {
                        Type = ChangeType.Removal,
                        Position = baseIndex,
                        BaseValue = line.Text
                    });
                    baseIndex += line.Text.Length;
                }
                else if (line.Type == DiffPlex.DiffBuilder.Model.ChangeType.Inserted)
                {
                    changes.Add(new ChangeInfo
                    {
                        Type = ChangeType.Addition,
                        Position = modifiedIndex,
                        ModifiedValue = line.Text
                    });
                    modifiedIndex += line.Text.Length;
                }
            }

            return changes;
        }

        private string ApplyHighlights(string text, params List<ChangeInfo>[] changeLists)
        {
            var sortedChanges = changeLists.SelectMany(list => list)
                                            .OrderBy(change => change.Position)
                                            .ToList();

            var highlightedFragments = new List<string>();
            int currentPosition = 0;

            foreach (var change in sortedChanges)
            {
                string highlightTag;
                string highlightContent;

                if (change.Type == ChangeType.Removal)
                {
                    highlightTag = "removed";
                    highlightContent = change.BaseValue;
                }
                else if (change.Type == ChangeType.Addition)
                {
                    highlightTag = "added";
                    highlightContent = change.ModifiedValue;
                }
                else
                {
                    continue; // Skip unchanged items
                }

                var highlightStartTag = $"<span class=\"{highlightTag}\">";
                var highlightEndTag = "</span>";

                // Append the unchanged text fragment
                if (change.Position > currentPosition)
                {
                    highlightedFragments.Add(text.Substring(currentPosition, change.Position - currentPosition));
                }

                // Append the highlighted text fragment
                highlightedFragments.Add($"{highlightStartTag}{highlightContent}{highlightEndTag}");

                currentPosition = change.Position + highlightContent.Length;
            }

            // Append any remaining unchanged text
            if (currentPosition < text.Length)
            {
                highlightedFragments.Add(text.Substring(currentPosition));
            }

            return string.Concat(highlightedFragments);
        }


    }
}
