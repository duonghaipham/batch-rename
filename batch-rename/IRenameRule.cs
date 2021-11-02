using System;
using System.Collections.Generic;
using System.Text;

namespace batch_rename
{
    interface IRenameRule
    {
        public string Rename(string original);
    }
}
