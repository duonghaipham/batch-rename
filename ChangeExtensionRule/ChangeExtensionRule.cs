using RenameRule;
using System;
using System.Text.RegularExpressions;

namespace ChangeExtensionRule
{
    public class ChangeExtensionRule: IRenameRule
    {
        private string extension;

        public ChangeExtensionRule(string extension)
        {
            this.extension = extension;
        }

        public string Rename(string original)
        {
            return Regex.Replace(original, @"\[.]\w+\g", '.' + extension);
        }
    }
}
