
using System.Threading;
using System.Threading.Tasks;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public interface IConsoleInputProcessor
    {
        Task ProcessAsync(
            ConsoleSession session,
            ConsoleInputAction action,
            CancellationToken cancellationToken = default);
    }
}