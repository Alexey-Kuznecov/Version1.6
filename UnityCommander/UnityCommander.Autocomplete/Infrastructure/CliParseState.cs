using UnityCommander.Autocomplete.Completion;
using UnityCommander.Autocomplete.Context.Descriptors;

namespace UnityCommander.Autocomplete.Infrastructure
{
    public sealed class CliParseState
    {
        public ICommandDescriptor? Command { get; }
        public IReadOnlyList<ParsedArgument> PositionalArguments { get; }
        public IReadOnlyList<ParsedFlag> Flags { get; }
        public CompletionKind ExpectedNext { get; }
        public int ArgumentIndex { get; }
        public CliError? Error { get; }

        public CliParseState(
            ICommandDescriptor? command,
            IReadOnlyList<ParsedArgument> positionalArguments,
            IReadOnlyList<ParsedFlag> flags,
            CompletionKind expectedNext,
            int argumentIndex,
            CliError? error)
        {
            Command = command;
            PositionalArguments = positionalArguments;
            Flags = flags;
            ExpectedNext = expectedNext;
            ArgumentIndex = argumentIndex;
            Error = error;
        }
    }
}
