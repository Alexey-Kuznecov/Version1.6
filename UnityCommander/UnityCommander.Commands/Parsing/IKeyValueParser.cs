
namespace UnityCommander.Commands.Parsing
{
    public interface IKeyValueParser
    {
        bool TryParse(
            string value,
            out string key,
            out string valuePart);
    }
}
