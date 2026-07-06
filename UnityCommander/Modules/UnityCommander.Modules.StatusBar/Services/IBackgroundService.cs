
namespace UnityCommander.Modules.StatusBar.Services
{
    public interface IBackgroundService
    {
        string Id { get; }

        string Name { get; }

        bool IsRunning { get; }

        Task StartAsync();

        Task StopAsync();
    }
}
