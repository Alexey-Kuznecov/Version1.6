
namespace UnityCommander.CLI.Input
{
    public sealed class ArgumentContext : InputContext
    {
        public string CommandName { get; }
        public IReadOnlyList<string> ExistingArguments { get; }
        public string PartialArgument { get; }

        // 🔥 ВАЖНО
        public int ReplaceStart { get; }
        public int ReplaceLength { get; }

        public ArgumentContext(
            TokenizationResult tokens,
            string commandName,
            IReadOnlyList<string> existingArgs,
            string partialArgument,
            int replaceStart,
            int replaceLength)
            : base(tokens)
        {
            CommandName = commandName;
            ExistingArguments = existingArgs;
            PartialArgument = partialArgument;
            ReplaceStart = replaceStart;
            ReplaceLength = replaceLength;
        }
    }
}
