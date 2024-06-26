﻿using text_compare.Models;

namespace text_compare.Core.Interface
{
    public interface ICompareText
    {
        TextComparisonResult CompareAndHighlightChanges(string baseText, string modifiedText, List<ChangeInfo> changes);
        List<ChangeInfo> CompareLineByLine(string baseText, string modifiedText);
        List<ChangeInfo> CompareWordByWord(string baseText, string modifiedText);
        TextComparisonResult CompareLetterByLetter(string baseText, string modifiedText);
    }
}
 