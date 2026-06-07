
using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Context
{
    public abstract class InputContext
    {
        public CliParseState ParseState { get; }

        protected InputContext(CliParseState parseState)
        {
            ParseState = parseState;
        }
    }
}
