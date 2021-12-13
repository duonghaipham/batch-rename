using RenameRule;
using System;

namespace AddPrefixRule
{
    public class AddPrefixRule : IRenameRule
    {
        private string prefix;

        public AddPrefixRule(string prefix)
        {
            this.prefix = prefix;
        }

        public string Rename(string original)
        {
            return $"{prefix}{original}";
        }
    }
}
