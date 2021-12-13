using RenameRule;
using System;

namespace AddSuffixRule
{
    public class AddSuffixRule : IRenameRule
    {
        private string suffix;

        public AddSuffixRule(string suffix)
        {
            this.suffix = suffix;
        }

        public string Rename(string original)
        {
            return $"{original}{suffix}";
        }
    }
}
