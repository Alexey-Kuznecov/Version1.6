
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCommander.Commands.Parsing
{
    public sealed class ArgumentValueParser : IArgumentValueParser
    {
        public IReadOnlyList<string> Parse(string value)
        {
            return value
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();
        }
    }
}
