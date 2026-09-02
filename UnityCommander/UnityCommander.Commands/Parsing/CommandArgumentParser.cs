
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
            var named =
                new Dictionary<string, List<string>>(
                    StringComparer.OrdinalIgnoreCase);

            var positional =
                new List<string>();

            foreach (var arg in arguments)
            {
                if (arg.StartsWith("--"))
                {
                    var parts = arg
                        .Substring(2)
                        .Split('=', 2);

                    var name = parts[0];

                    var value = parts.Length > 1
                        ? parts[1]
                        : "true";

                    if (!named.TryGetValue(name, out var values))
                    {
                        values = new List<string>();
                        named[name] = values;
                    }

                    values.Add(value);
                }
                else
                {
                    positional.Add(arg);
                }
            }

            return new ArgumentCollection(
                named,
                positional);
        }
    }
}
