
using UnityCommander.CLI.Core;

namespace UnityCommander.CLI.Infrastructure
{
    public interface IConsoleActivity : IDisposable
    {
        void Update(Action<IConsoleActivityState> update);
    }
}
