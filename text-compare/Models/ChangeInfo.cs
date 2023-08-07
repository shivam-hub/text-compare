using static text_compare.Constants.Constants;

namespace text_compare.Models
{
    public class ChangeInfo
    {
        public ChangeType Type { get; set; }
        public int Position { get; set; }
        public string BaseValue { get; set; }
        public string ModifiedValue { get; set; }
    }
}
