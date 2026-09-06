
using System.Collections.Generic;

namespace UnityCommander.Commands.Parsing
{
    public interface IArgumentValueParser
    {
        IReadOnlyList<string> Parse(string value);
    }
}
