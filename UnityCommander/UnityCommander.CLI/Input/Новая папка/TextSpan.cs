using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.CLI.Input
{
    public readonly struct TextSpan
    {
        public int Start { get; }
        public int Length { get; }

        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }
    }
}
