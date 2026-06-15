
using System.Threading;
using System.Threading.Tasks;

namespace UnityCommander.Services.Background
{
    public interface IBackgroundService
    {
        Task RunAsync(CancellationToken token);
    }
}
