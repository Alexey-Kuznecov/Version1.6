
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Input;

namespace UnityCommander.Autocomplete.Infrastructure
{
    public sealed class CliParseState
    {
        public ICommandDescriptor? Command { get; }
        public IReadOnlyList<ParsedArgument> PositionalArguments { get; }
        public IReadOnlyList<ParsedFlag> Flags { get; }
        public IReadOnlyList<IPositionalArgumentDescriptor> AvailableArguments { get; }
        public IReadOnlyList<IFlagDescriptor> AvailableFlags { get; }
        public CompletionKind ExpectedNext { get; }
        public int ArgumentIndex { get; }
        public CliError? Error { get; }
        public bool IsComplete { get; }
        public bool IsEditingToken { get; }

        // Новые свойства для автокомплита
        public int ReplaceStart { get; }
        public int ReplaceLength { get; }
        public string PartialValue { get; }
        public InputToken? CurrentToken { get; }

        public CliParseState(
            ICommandDescriptor? command,
            IReadOnlyList<ParsedArgument> positionalArguments,
            IReadOnlyList<ParsedFlag> flags,
            IReadOnlyList<IPositionalArgumentDescriptor> availableArguments,
            IReadOnlyList<IFlagDescriptor> availableFlags,
            CompletionKind expectedNext,
            int argumentIndex,
            CliError? error,
            InputToken? currentToken = null,
            int replaceStart = 0,
            int replaceLength = 0,
            string partialValue = "",
            bool isEditingToken = false)
        {
            Command = command;
            PositionalArguments = positionalArguments;
            Flags = flags;
            AvailableArguments = availableArguments;
            AvailableFlags = availableFlags;
            ExpectedNext = expectedNext;
            ArgumentIndex = argumentIndex;
            Error = error;
            IsComplete = error == null && expectedNext == CompletionKind.Nothing;

            CurrentToken = currentToken;
            ReplaceStart = replaceStart;
            ReplaceLength = replaceLength;
            PartialValue = partialValue;
            IsEditingToken = isEditingToken;
        }
    }
}
