
using System.Collections.Generic;

namespace UnityCommander.Commands.Parsing
{
    public interface ICommandArgumentParser
    {
        IArgumentCollection Parse(
            IEnumerable<string> arguments);
    }
}
