namespace text_compare.Models
{
    public class TextComparisonResult
    {
        public int AddedCount { get; set; }
        public int RemovedCount { get; set; }
        public List<ChangeInfo> Changes { get; set; }
    }
}
