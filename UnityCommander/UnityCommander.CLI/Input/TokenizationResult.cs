using Newtonsoft.Json.Linq;

namespace UnityCommander.CLI.Input
{
    public sealed class TokenizationResult
    {
        public IReadOnlyList<InputToken>? Tokens { get; init; }
        public int CaretPosition { get; init; }

        // 👇 ВОТ ОНИ
        public int CurrentTokenStart { get; init; }
        public int CurrentTokenLength { get; init; }
        public string? CurrentTokenValue { get; init; }
    }
}
