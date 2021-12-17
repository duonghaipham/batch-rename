using Contract;

namespace ChangeExtensionRule
{
    public class ChangeExtensionRuleParser : IRenameRuleParser
    {
        public string Name => "ChangeExtension";
        public string Title => "Change extension";
        public bool IsPlugAndPlay => false;

        public IRenameRule Parse(string line)
        {
            throw new System.NotImplementedException();
        }
    }
}
