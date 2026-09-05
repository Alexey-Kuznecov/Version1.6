
namespace UnityCommander.Commands.Parsing
{
    public sealed class KeyValueParser : IKeyValueParser
    {
        public bool TryParse(
            string value,
            out string key,
            out string valuePart)
        {
            var separator = value.IndexOf('=');

            if (separator <= 0)
            {
                key = string.Empty;
                valuePart = string.Empty;
                return false;
            }

            key = value[..separator];
            valuePart = value[(separator + 1)..];

            return true;
        }
    }
}
