
namespace UnityCommander.CLI.Input
{
    public sealed class CommandNameContext : InputContext
    {
        public string PartialCommand { get; }
        public int ReplaceStart { get; }
        public int ReplaceLength { get; }

        public CommandNameContext(TokenizationResult tokens, string partial)
            : base(tokens)
        {
            PartialCommand = partial;
        }
    }
}
