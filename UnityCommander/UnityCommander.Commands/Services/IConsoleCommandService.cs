

using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.Commands.Parsing;

namespace UnityCommander.Commands.Services
{
    public interface IConsoleCommandService
    {
        Task RunAsync(
            IConsoleOutput output,
            IArgumentCollection args,
            CancellationToken token);
    }
}
