
using UnityCommander.CLI.Core;
using UnityCommander.Commands.Performance;

namespace UnityCommander.Commands.Rendering
{
    public sealed class SystemStatsRenderer
      : IConsoleRenderer<SystemStats>
    {
        public void Render(
          IConsoleOutput output,
          SystemStats stats)
        {
            output.Clear();

            output.WriteLine("╔══════════════════════════════════════╗");
            output.WriteLine("║           SYSTEM STATS               ║");
            output.WriteLine("╚══════════════════════════════════════╝");

            output.WriteLine($"  {"CPU Usage",-22}: {stats.CpuUsage,8:F2} %");
            output.WriteLine($"  {"Available Memory",-22}: {stats.AvailableMemoryMb,8:F2} MB");
            output.WriteLine($"  {"Disk Reads/sec",-22}: {stats.DiskReadsPerSec,8:F2}");
            output.WriteLine($"  {"Disk Writes/sec",-22}: {stats.DiskWritesPerSec,8:F2}");
            output.WriteLine($"  {"Disk Transfers/sec",-22}: {stats.DiskTransfersPerSec,8:F2}");
        }
    }
}
