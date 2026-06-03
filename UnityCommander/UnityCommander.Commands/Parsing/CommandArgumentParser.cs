
using System;
using System.Collections.Generic;

namespace UnityCommander.Commands.Parsing
{
    public sealed class CommandArgumentParser
     : ICommandArgumentParser
    {
        public IArgumentCollection Parse(
            IEnumerable<string> arguments)
        {
            var dict =
                new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (var arg in arguments)
            {
                if (!arg.StartsWith("--"))
                    continue;

                var parts =
                    arg.Substring(2)
                        .Split('=', 2);

                dict[parts[0]] =
                    parts.Length > 1
                        ? parts[1]
                        : "true";
            }

            return new ArgumentCollection(dict);
        }
    }
}
