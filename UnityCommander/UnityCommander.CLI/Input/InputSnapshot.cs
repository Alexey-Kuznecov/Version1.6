using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.CLI.Input
{
    public class InputSnapshot
    {
        public string Text { get; }
        public IReadOnlyList<string> Tokens { get; }

        public InputSnapshot(string text, IReadOnlyList<string> tokens)
        {
            Text = text;
            Tokens = tokens;
        }
    }
}
