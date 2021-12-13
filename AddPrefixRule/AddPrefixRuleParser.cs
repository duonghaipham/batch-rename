using Contract;
using RenameRule;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddPrefixRule
{
    public class AddPrefixRuleParser : IRenameRuleParser
    {
        public string Name => "Add prefix";

        public IRenameRule Parse(string line)
        {
            string[] tokens = line.Split(new string[] { "AddPrefix " }, StringSplitOptions.None);

            string prefix = tokens[1].Replace("\"", "");
            IRenameRule rule = new AddPrefixRule(prefix);

            return rule;
        }
    }
}
