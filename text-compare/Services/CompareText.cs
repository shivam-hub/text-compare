using text_compare.Core.Interface;
using text_compare.Models;
using static text_compare.Constants.Constants;
using DiffPlex;
using DiffPlex.DiffBuilder;

namespace text_compare.Services
{
    public class CompareText : ICompareText
    {
        public TextComparisonResult CompareAndHighlightChanges(string baseText, string modifiedText)
        {
            var lineChanges = CompareLineByLine(baseText, modifiedText);
            var wordChanges = CompareWordByWord(baseText, modifiedText);
            var letterChanges = CompareLetterByLetter(baseText, modifiedText);

            // Aggregate the changes and generate highlighted texts
            var baseWithHighlights = ApplyHighlights(baseText, lineChanges, wordChanges, letterChanges);
            var modifiedWithHighlights = ApplyHighlights(modifiedText, lineChanges, wordChanges, letterChanges);

            return new TextComparisonResult
            {
                BaseTextWithHighlights = baseWithHighlights,
                ModifiedTextWithHighlights = modifiedWithHighlights,
                RemovalCount = lineChanges.Count(c => c.Type == ChangeType.Removal),
                AdditionCount = lineChanges.Count(c => c.Type == ChangeType.Addition)
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

        public List<ChangeInfo> CompareLetterByLetter(string baseText, string modifiedText)
        {
            var baseLetters = baseText.ToCharArray();
            var modifiedLetters = modifiedText.ToCharArray();

            return CompareSequences(baseLetters, modifiedLetters);
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

            int offset = 0;
            foreach (var change in sortedChanges)
            {
                if (change.Type == ChangeType.Removal)
                {
                    text = text.Insert(change.Position + offset, $"<span class=\"removed\">{change.BaseValue}</span>");
                    offset += $"<span class=\"removed\">{change.BaseValue}</span>".Length;
                }
                else if (change.Type == ChangeType.Addition)
                {
                    text = text.Insert(change.Position + offset, $"<span class=\"added\">{change.ModifiedValue}</span>");
                    offset += $"<span class=\"added\">{change.ModifiedValue}</span>".Length;
                }
            }

            return text;
        }
    }
}
