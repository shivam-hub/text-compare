using text_compare.Models;

namespace text_compare.Core.Interface
{
    public interface ICompareText
    {
        public TextComparisonResult CompareWordByWord(string baseText, string modifiedText);
    }
}
 