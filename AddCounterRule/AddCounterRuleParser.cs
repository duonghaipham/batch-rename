using Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddCounterRule
{
    public class AddCounterRuleParser : IRenameRuleParser
    {
        public string Name => "AddCounter";

        public string Title => "Add counter";

        public bool IsPlugAndPlay => false;

        public IRenameRule Parse(string line)
        {

            throw new NotImplementedException();
        }
    }
}
