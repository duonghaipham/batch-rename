using Contract;

namespace RemoveSpaceRule
{
    public class RemoveSpaceRuleParser : IRenameRuleParser
    {
        public string Name => "RemoveSpace";
        public string Title => "Remove space";
        public bool IsPlugAndPlay => true;

        public IRenameRule Parse(string line)
        {
            IRenameRule rule = new RemoveSpaceRule();

            return rule;
        }
    }
}
