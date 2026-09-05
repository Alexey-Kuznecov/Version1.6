
using System;
using System.Collections.Generic;
using System.Linq;

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

            var args = arguments.ToArray();

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (!arg.StartsWith("--"))
                {
                    positional.Add(arg);
                    continue;
                }

                var name = arg.Substring(2);

                string? value = null;

                var separator = name.IndexOf('=');

                if (separator >= 0)
                {
                    value = name[(separator + 1)..];
                    name = name[..separator];
                }
                else if (i + 1 < args.Length &&
                         !args[i + 1].StartsWith("--"))
                {
                    value = args[++i];
                }

                if (!named.TryGetValue(name, out var values))
                {
                    values = new List<string>();
                    named[name] = values;
                }

                if (value != null)
                    values.Add(value);
            }

            return new ArgumentCollection(
                named,
                positional);
        }
    }
}
