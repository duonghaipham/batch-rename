using Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllLowerRule
{
    public class AllLowerRuleParser : IRenameRuleParser
    {
        public string Name => "AllLower";
        public string Title => "All lower";
        public bool IsPlugAndPlay => true;

        public IRenameRule Parse(string line)
        {
            IRenameRule rule = new AllLowerRule();

            return rule;
        }
    }
}
