
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Context
{
    public sealed class ArgumentContext : InputContext
    {
        public int CurrentTokenIndex => ParseState.ArgumentIndex;
        public IReadOnlyList<IPositionalArgumentDescriptor> AvailableArguments => ParseState.AvailableArguments;
        public IReadOnlyList<IFlagDescriptor> AvailableFlags => ParseState.AvailableFlags;

        public CompletionKind ExpectedNext => ParseState.ExpectedNext;

        public ArgumentContext(CliParseState parseState) : base(parseState) { }
    }
}