using Contract;
using System;

namespace AddCounterRule
{
    public class AddCounterRule : IRenameRule
    {
        public int Start { get; }
        public int Step { get; }
        public int NumberOfDigits { get; }

        public AddCounterRule(int start, int step, int numberOfDigits)
        {
            Start = start;
            Step = step;
            NumberOfDigits = numberOfDigits;
        }

        public string Rename(string original)
        {
            throw new NotImplementedException();
        }
    }
}
