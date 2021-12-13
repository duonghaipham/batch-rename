using RenameRule;

namespace Contract
{
    public interface IRenameRuleParser
    {
        public string Name { get; }
        IRenameRule Parse(string line);
    }
}
