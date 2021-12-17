using Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReplaceRule
{
    public class ReplaceRuleParser : IRenameRuleParser
    {
        public string Name => "Replace";

        public string Title => "Replace";

        public bool IsPlugAndPlay => false;

        public IRenameRule Parse(string line)
        {
            throw new NotImplementedException();
        }
    }
}
