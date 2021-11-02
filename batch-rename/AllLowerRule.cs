using System;
using System.Collections.Generic;
using System.Text;

namespace batch_rename
{
    class AllLowerRule : IRenameRule
    {
        public string Rename(string original)
        {
            return original.ToLower();
        }
    }
}
