
namespace UnityCommander.Abstractions.Background
{
    public interface IBackgroundService
    {
        string Id { get; }

        string Name { get; }

        bool IsRunning { get; }

        bool AutoStart { get; }

        string OwnerId { get; }

        Task RunAsync(CancellationToken token);

        Task StopAsync();
    }
}
