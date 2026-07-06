
namespace UnityCommander.Abstractions.Background
{
    public interface IBackgroundService
    {
        Task RunAsync(CancellationToken token);
    }
}
