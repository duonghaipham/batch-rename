using Contract;
using RenameRule;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddSuffixRule
{
    public class AddSuffixRuleParser : IRenameRuleParser
    {
        public string Name => "Add suffix";

        public IRenameRule Parse(string line)
        {
            string[] tokens = line.Split(new string[] { "AddSuffix " }, StringSplitOptions.None);

            string suffix = tokens[1].Replace("\"", "");
            IRenameRule rule = new AddSuffixRule(suffix);

            return rule;
        }
    }
}
