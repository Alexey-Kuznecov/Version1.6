
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.Modules.BottomPanel.Console;

namespace UnityCommander.Modules.BottomPanel.Services
{
    public interface IStartupCommandRunner
    {
        Task RunStartupCommands(
             ConsoleSession session,
             CancellationToken cancellationToken);
    }
}
