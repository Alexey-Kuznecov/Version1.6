
namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    public interface ICliParseStateBuilder
    {
        CliParseState Build(InputStatus status);
    }
}
