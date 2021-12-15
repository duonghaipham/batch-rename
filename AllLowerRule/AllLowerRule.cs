using Contract;
using System;
using System.Text.RegularExpressions;

namespace AllLowerRule
{
    public class AllLowerRule : IRenameRule
    {
        public string Rename(string original)
        {
            string newName = original;

            newName = newName.ToLower();
            newName = Regex.Replace(newName, @"\s+", "");

            return newName;
        }
    }
}
