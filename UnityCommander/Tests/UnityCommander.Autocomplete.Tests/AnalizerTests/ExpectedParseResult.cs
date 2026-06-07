
namespace UnityCommander.Autocomplete.Tests.AnalizerTests
{
    public record ExpectedParseResult(
        string? CommandName,
        string[] Flags,
        (string Name, string? Value)[] FlagValues,
        string[] PositionalValues,
        bool IsComplete,
        bool HasError
    );
}
