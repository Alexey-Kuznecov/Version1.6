
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
                new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase);

            var positional =
                new List<string>();

            foreach (var arg in arguments)
            {
                if (arg.StartsWith("--"))
                {
                    var parts =
                        arg.Substring(2)
                            .Split('=', 2);

                    named[parts[0]] =
                        parts.Length > 1
                            ? parts[1]
                            : "true";
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
