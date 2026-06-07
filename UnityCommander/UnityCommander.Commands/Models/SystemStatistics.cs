
namespace UnityCommander.Commands.Models
{
    public sealed class SystemStatistics
    {
        public float CpuUsage { get; init; }

        public float MemoryUsage { get; init; }

        public float DiskRead { get; init; }

        public float DiskWrite { get; init; }
    }
}
