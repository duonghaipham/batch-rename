using RenameRule;
using System;

namespace RemoveSpaceRule
{
    public class RemoveSpaceRule: IRenameRule
    {
        public string Rename(string original)
        {
            return original.Trim();
        }
    }
}
