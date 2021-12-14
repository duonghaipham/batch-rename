using Contract;
using RenameRule;
using System;

namespace AddPrefixRule
{
    public class AddPrefixRuleParser : IRenameRuleParser
    {
        public string Name => "AddPrefix";

        public IRenameRule Parse(string line)
        {
            string[] tokens = line.Split(new string[] { "AddPrefix " }, StringSplitOptions.None);

            string prefix = tokens[1].Replace("\"", "");
            IRenameRule rule = new AddPrefixRule(prefix);

            return rule;
        }
    }
}
