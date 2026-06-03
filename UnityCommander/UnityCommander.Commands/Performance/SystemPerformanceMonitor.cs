using System;
using System.Diagnostics;

namespace UnityCommander.Commands.Performance
{
    public class SystemPerformanceMonitor : IDisposable
    {
        private readonly PerformanceCounter cpuCounter;
        private readonly PerformanceCounter memoryCounter;
        private readonly PerformanceCounter diskReadCounter;
        private readonly PerformanceCounter diskWriteCounter;
        private readonly PerformanceCounter diskTransferCounter;

        public SystemPerformanceMonitor(string disk = "_Total")
        {
            var instance = PhysicalDiskResolver.Resolve(disk);

            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            memoryCounter = new PerformanceCounter("Memory", "Available MBytes");

            diskReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Reads/sec", instance);
            diskWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Writes/sec", instance);
            diskTransferCounter = new PerformanceCounter("PhysicalDisk", "Disk Transfers/sec", instance);
        }

        public SystemStats GetStats()
        {
            return new SystemStats
            {
                CpuUsage = cpuCounter.NextValue(),
                AvailableMemoryMb = memoryCounter.NextValue(),
                DiskReadsPerSec = diskReadCounter.NextValue(),
                DiskWritesPerSec = diskWriteCounter.NextValue(),
                DiskTransfersPerSec = diskTransferCounter.NextValue()
            };
        }

        public void Dispose()
        {
            cpuCounter.Dispose();
            memoryCounter.Dispose();
            diskReadCounter.Dispose();
            diskWriteCounter.Dispose();
            diskTransferCounter.Dispose();
        }
    }

    public class SystemStats
    {
        public float CpuUsage { get; set; }
        public float AvailableMemoryMb { get; set; }
        public float DiskReadsPerSec { get; set; }
        public float DiskWritesPerSec { get; set; }
        public float DiskTransfersPerSec { get; set; }
    }
}
