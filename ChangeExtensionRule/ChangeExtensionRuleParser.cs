using Contract;

namespace ChangeExtensionRule
{
    public class ChangeExtensionRuleParser : IRenameRuleParser
    {
        public string Name => "ChangeExtension";

        public IRenameRule Parse(string line)
        {
            throw new System.NotImplementedException();
        }
    }
}
