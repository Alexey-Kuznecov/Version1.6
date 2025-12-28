
using UnityCommander.Abstractions.Completion;

namespace UnityCommander.Autocomplete.Infrastructure
{
    public sealed class CliParseState
    {
        public ICommandDescriptor? Command { get; }
        public IReadOnlyList<ParsedArgument> PositionalArguments { get; }
        public IReadOnlyList<ParsedFlag> Flags { get; }
        public IReadOnlyList<IFlagDescriptor> AvailableFlags { get; }
        public IReadOnlyList<IPositionalArgumentDescriptor> AvailableArguments { get; }
        public CompletionKind ExpectedNext { get; }
        public int ArgumentIndex { get; }
        public CliError? Error { get; }
        public bool IsComplete { get; }
        
        public CliParseState(
            ICommandDescriptor? command,
            IReadOnlyList<ParsedArgument> positionalArguments,
            IReadOnlyList<ParsedFlag> flags,
            IReadOnlyList<IPositionalArgumentDescriptor> availableArguments,
            IReadOnlyList<IFlagDescriptor> availableFlags,
            CompletionKind expectedNext,
            int argumentIndex,
            CliError? error)
        {
            Command = command;
            PositionalArguments = positionalArguments;
            Flags = flags;
            AvailableArguments = availableArguments;
            AvailableFlags = availableFlags;
            ExpectedNext = expectedNext;
            ArgumentIndex = argumentIndex;
            Error = error;
            IsComplete =
                Error == null &&
                ExpectedNext == CompletionKind.None &&
                !AvailableFlags.Any() &&
                !AvailableArguments.Any();
        }
    }
}
