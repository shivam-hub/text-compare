using static text_compare.Constants.Constants;

namespace text_compare.Models
{
    public class ChangeInfo
    {
        public TextChangeType Type { get; set; }
        public string Element { get; set; }
        public int Position { get; set; }
    }
}
