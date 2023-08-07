namespace text_compare.Models
{
    public class TextComparisonResult
    {
        public string BaseTextWithHighlights { get; set; }
        public string ModifiedTextWithHighlights { get; set; }
        public int RemovalCount { get; set; }
        public int AdditionCount { get; set; }
    }
}
