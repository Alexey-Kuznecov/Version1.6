using Newtonsoft.Json.Linq;

namespace UnityCommander.CLI.Input
{
    public sealed class TokenizationResult
    {
        public IReadOnlyList<InputToken>? Tokens { get; init; }
        public int CaretPosition { get; init; }
    }
}
