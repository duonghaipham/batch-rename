using System;

namespace RenameRule
{
    public interface IRenameRule
    {
        public string Rename(string original);
    }
}
