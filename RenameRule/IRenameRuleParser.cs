namespace Contract
{
    public interface IRenameRuleParser
    {
        public string Name { get; }
        public string Title { get; }
        public bool IsPlugAndPlay { get; }
        IRenameRule Parse(string line);
    }
}
