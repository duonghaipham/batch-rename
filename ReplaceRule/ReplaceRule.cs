using RenameRule;
using System;
using System.Collections.Generic;

namespace ReplaceRule
{
    public class ReplaceRule: IRenameRule
    {
        private List<string> needles;
        private string replacer;

        public ReplaceRule(List<string> needles, string replacer)
        {
            this.needles = needles;
            this.replacer = replacer;
        }

        public string Rename(string original)
        {
            string result = original;

            foreach (string needle in needles)
            {
                result = result.Replace(needle, replacer);
            }

            return result;
        }
    }
}
