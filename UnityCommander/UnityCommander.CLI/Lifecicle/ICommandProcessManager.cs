
namespace UnityCommander.CLI.Lifecicle
{
    public interface ICommandProcessManager
    {
        Guid Start(Func<CancellationToken, Task> task);
        void Stop(Guid id);
        void StopAll();
    }
}
