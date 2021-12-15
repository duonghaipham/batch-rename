using Contract;

namespace CaseRule
{
    public class CaseRule : IRenameRule
    {
        public string Rename(string original)
        {
            return original.ToLower();
        }
    }
}
