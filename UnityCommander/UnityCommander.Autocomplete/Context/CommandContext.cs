
using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Context
{
    public sealed class CommandContext : InputContext
    {
        public CommandContext(CliParseState parseState) : base(parseState) { }
    }
}
