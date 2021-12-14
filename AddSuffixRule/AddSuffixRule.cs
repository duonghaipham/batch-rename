using Contract;

namespace AddSuffixRule
{
    public class AddSuffixRule : IRenameRule
    {
        public string Suffix;

        public AddSuffixRule(string suffix)
        {
            Suffix = suffix;
        }

        public string Rename(string original)
        {
            return $"{original}{Suffix}";
        }
    }
}
