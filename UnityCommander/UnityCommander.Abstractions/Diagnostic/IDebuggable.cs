
namespace UnityCommander.Abstractions.Diagnostic
{
    public interface IDebuggable<TState>
    {
        TState GetDebugState();
    }
}
