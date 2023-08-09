using text_compare.Core.Interface;
using text_compare.Models;
using static text_compare.Constants.Constants;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace text_compare.Services
{
    public class CompareText : ICompareText
    {
        public TextComparisonResult CompareWordByWord(string baseText, string modifiedText)
        {
            var differ = new Differ();
            var wordBuilder = new SideBySideDiffBuilder(differ);
            var diffResult = wordBuilder.BuildDiffModel(baseText, modifiedText);

            var changes = new List<ChangeInfo>();
            int addedCount = 0;
            int removedCount = 0;

            foreach (var line in diffResult.NewText.Lines)
            {
                var lineChanges = ProcessDiffPieces(line.SubPieces);

                addedCount += lineChanges.Count(change => change.Type == TextChangeType.Addition);
                removedCount += lineChanges.Count(change => change.Type == TextChangeType.Removal);

                changes.AddRange(lineChanges);
            }

            foreach (var line in diffResult.OldText.Lines)
            {
                var lineChanges = ProcessDiffPieces(line.SubPieces);

                addedCount += lineChanges.Count(change => change.Type == TextChangeType.Addition);
                removedCount += lineChanges.Count(change => change.Type == TextChangeType.Removal);

                changes.AddRange(lineChanges);
            }

            return new TextComparisonResult
            {
                AddedCount = addedCount,
                RemovedCount = removedCount,
                Changes = changes
            };
        }

        private List<ChangeInfo> ProcessDiffPieces(IEnumerable<DiffPiece> diffPieces)
        {
            var changes = new List<ChangeInfo>();

            foreach (var diffPiece in diffPieces)
            {
                TextChangeType changeType;

                switch (diffPiece.Type)
                {
                    case ChangeType.Inserted:
                        changeType = TextChangeType.Addition;
                        break;
                    case ChangeType.Deleted:
                        changeType = TextChangeType.Removal;
                        break;
                    default:
                        continue;
                }

                changes.Add(new ChangeInfo
                {
                    Type = changeType,
                    Element = diffPiece.Text,
                    Position = Convert.ToInt32(diffPiece.Position)
                });
            }

            return changes;
        }

    };

}
