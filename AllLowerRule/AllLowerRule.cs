using RenameRule;
using System;

namespace AllLowerRule
{
    public class AllLowerRule : IRenameRule
    {
        public string Rename(string original)
        {
            return original.ToLower();
        }
    }
}
