using Contract;
using System.IO;

namespace RemoveSpaceRule
{
    public class RemoveSpaceRule: IRenameRule
    {
        public string Rename(string original)
        {
            return Path.GetFileNameWithoutExtension(original).Trim() + Path.GetExtension(original).Trim();
        }
    }
}
