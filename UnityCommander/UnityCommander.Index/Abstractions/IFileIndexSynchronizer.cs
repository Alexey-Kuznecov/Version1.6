
namespace UnityCommander.Index.Abstractions
{
    public interface IFileIndexSynchronizer
    {
        Task StartAsync(
            string path,
            CancellationToken cancellationToken = default);

        Task StopAsync(
            CancellationToken cancellationToken = default);
    }
}
