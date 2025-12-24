using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.CLI.Autocomplete
{
    public interface IAutoCompleteArgumentsProvider
    {
        IEnumerable<string> GetArgumentSuggestions(string[] currentArgs);
    }
}
