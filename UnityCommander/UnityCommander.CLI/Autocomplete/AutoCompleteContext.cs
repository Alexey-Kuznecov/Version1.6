using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.CLI.Autocomplete
{
    public class AutoCompleteContext
    {
        public string? RawInput { get; }
        public string? CommandName { get; }
        public IReadOnlyList<string>? Arguments { get; }
        public int CaretIndex { get; }
    }
}
