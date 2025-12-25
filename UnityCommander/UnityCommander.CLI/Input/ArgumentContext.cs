
namespace UnityCommander.CLI.Input
{
    public sealed class ArgumentContext : InputContext
    {
        public string CommandName { get; }
        public string PartialArgument { get; }
        public IReadOnlyList<string> ExistingArguments { get; }
        public int ReplaceStart { get; }
        public int ReplaceLength { get; }

        public ArgumentContext(
            TokenizationResult tokens,
            string commandName,
            IReadOnlyList<string> existingArguments,
            string partialArgument,
            int replaceStart,
            int replaceLength
        ) : base(tokens)
        {
            CommandName = commandName;
            ExistingArguments = existingArguments;
            PartialArgument = partialArgument;
            ReplaceStart = replaceStart;
            ReplaceLength = replaceLength;
        }
    }
}