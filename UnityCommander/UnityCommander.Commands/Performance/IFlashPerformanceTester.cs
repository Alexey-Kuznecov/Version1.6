
using System.Threading;
using System.Threading.Tasks;

namespace UnityCommander.Commands.Performance
{
    public interface IFlashPerformanceTester
    {
        Task<FlashPerformanceResult> TestAsync(string driveLetter, int fileSizeBytes, int iterations, CancellationToken cancellationToken = default);
    }
}
