
namespace UnityCommander.Test
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// The hdd counter.
    /// </summary>
    public class HDDCounter : IDisposable
    {
        #region Common hard drive performance counter.

        /// <summary>
        /// The cpu performance counter.
        /// </summary>
        private readonly PerformanceCounter cpuPerformanceCounter = new PerformanceCounter();

        /// <summary>
        /// The memory performance counter.
        /// </summary>
        private readonly PerformanceCounter memoryPerformanceCounter = new PerformanceCounter();

        /// <summary>
        /// The disk reads performance counter.
        /// </summary>
        private readonly PerformanceCounter diskReadsPerformanceCounter = new PerformanceCounter();

        /// <summary>
        /// The disk writes performance counter.
        /// </summary>
        private readonly PerformanceCounter diskWritesPerformanceCounter = new PerformanceCounter();

        /// <summary>
        /// The disk transfers performance counter.
        /// </summary>
        private readonly PerformanceCounter diskTransfersPerformanceCounter = new PerformanceCounter();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="HDDCounter"/> class.
        /// </summary>
        public HDDCounter() => this.InitPerformanceCounters(); // This can be put in class constructor also.. 

        /// <summary>
        /// Gets or sets the drive letter.
        /// </summary>
        public string DriveLetter { get; set; } = "_Total";

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.cpuPerformanceCounter.Dispose();
            this.cpuPerformanceCounter.Dispose();
            this.cpuPerformanceCounter.Dispose();
            this.memoryPerformanceCounter.Dispose();
            this.memoryPerformanceCounter.Dispose();
            this.diskReadsPerformanceCounter.Dispose();
            this.diskReadsPerformanceCounter.Dispose();
            this.diskReadsPerformanceCounter.Dispose();
            this.diskWritesPerformanceCounter.Dispose();
            this.diskWritesPerformanceCounter.Dispose();
            this.diskWritesPerformanceCounter.Dispose();
            this.diskTransfersPerformanceCounter.Dispose();
            this.diskTransfersPerformanceCounter.Dispose();
            this.diskTransfersPerformanceCounter.Dispose();
        }

        /// <summary>
        /// The next.
        /// </summary>
        public void Next()
        {
            this.cpuPerformanceCounter.NextValue();
            this.cpuPerformanceCounter.NextValue();
            this.cpuPerformanceCounter.NextValue();
            this.memoryPerformanceCounter.NextValue();
            this.memoryPerformanceCounter.NextValue();
            this.diskReadsPerformanceCounter.NextValue();
            this.diskReadsPerformanceCounter.NextValue();
            this.diskReadsPerformanceCounter.NextValue();
            this.diskWritesPerformanceCounter.NextValue();
            this.diskWritesPerformanceCounter.NextValue();
            this.diskWritesPerformanceCounter.NextValue();
            this.diskTransfersPerformanceCounter.NextValue();
            this.diskTransfersPerformanceCounter.NextValue();
            this.diskTransfersPerformanceCounter.NextValue();
        }

        /// <summary>
        /// The print performance data.
        /// </summary>
        public void PrintPerformanceData()
        {
            string currentCpuUsage = "CPU Usage : " + this.cpuPerformanceCounter.NextValue() + " %" + Environment.NewLine;
            string currentMemoryUsage = "Memory Usage : " + this.memoryPerformanceCounter.NextValue() + " Mb" + Environment.NewLine;
            string currentDiskReads = "Disk reads / sec : " + this.diskReadsPerformanceCounter.NextValue() + Environment.NewLine;
            string currentDiskWrites = "Disk writes / sec : " + this.diskWritesPerformanceCounter.NextValue() + Environment.NewLine;
            string currentDiskTransfers = "Disk transfers / sec : " + this.diskTransfersPerformanceCounter.NextValue() + Environment.NewLine;

            Console.Write("{0}{1}{2}{3}{4}", currentCpuUsage, currentMemoryUsage, currentDiskReads, currentDiskWrites, currentDiskTransfers);
        }

        /// <summary>
        /// The init performance counters.
        /// </summary>
        private void InitPerformanceCounters()
        {
            this.cpuPerformanceCounter.CategoryName = "Processor";
            this.cpuPerformanceCounter.CounterName = "% Processor Time";
            this.cpuPerformanceCounter.InstanceName = this.DriveLetter;

            this.memoryPerformanceCounter.CategoryName = "Memory";
            this.memoryPerformanceCounter.CounterName = "Available MBytes";

            this.diskReadsPerformanceCounter.CategoryName = "PhysicalDisk";
            this.diskReadsPerformanceCounter.CounterName = "Disk Reads/sec";
            this.diskReadsPerformanceCounter.InstanceName = this.DriveLetter;

            this.diskWritesPerformanceCounter.CategoryName = "PhysicalDisk";
            this.diskWritesPerformanceCounter.CounterName = "Disk Writes/sec";
            this.diskWritesPerformanceCounter.InstanceName = this.DriveLetter;

            this.diskTransfersPerformanceCounter.CategoryName = "PhysicalDisk";
            this.diskTransfersPerformanceCounter.CounterName = "Disk Transfers/sec";
            this.diskTransfersPerformanceCounter.InstanceName = this.DriveLetter;
        }
    }
}
