using System;
using System.Collections.Generic;
using System.Text;

namespace batch_rename
{
    class AddPrefixRule : IRenameRule
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
