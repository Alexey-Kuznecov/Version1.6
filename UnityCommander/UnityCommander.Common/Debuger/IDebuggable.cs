

namespace UnityCommander.Common.Debugger
{
    public interface IDebuggable<TState>
    {
        TState GetDebugState();
    }
}
