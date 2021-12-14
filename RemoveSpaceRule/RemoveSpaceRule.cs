using Contract;

namespace RemoveSpaceRule
{
    public class RemoveSpaceRule: IRenameRule
    {
        public string Rename(string original)
        {
            return original.Trim();
        }
    }
}
