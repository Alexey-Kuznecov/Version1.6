
namespace UnityCommander.Test
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// The process counter.
    /// </summary>
    public class ProcessCounter : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessCounter"/> class.
        /// </summary>
        /// <param name="instance"> The instance. </param>
        public ProcessCounter(Process instance)
        {
            this.ProcessInstance = instance;
            this.InitPerformanceData();
        }

        /// <summary>
        /// Gets the process name.
        /// </summary>
        public Process ProcessInstance { get; }

        /// <summary>
        /// Gets the processor time.
        /// </summary>
        public PerformanceCounter ProcessorTime { get; private set; }

        /// <summary>
        /// Gets the creating process id.
        /// </summary>
        public PerformanceCounter CreatingProcessId { get; private set; }

        /// <summary>
        /// Gets the id process.
        /// </summary>
        public PerformanceCounter IDProcess { get; private set; }

        /// <summary>
        /// Gets the IO write bytes/sec.
        /// </summary>
        public PerformanceCounter IOWriteBytes { get; private set; }
        
        /// <summary>
        /// Gets the IO write operations/sec.
        /// </summary>
        public PerformanceCounter IOWriteOperation { get; private set; }

        /// <summary>
        /// Gets the IO read bytes/sec.
        /// </summary>
        public PerformanceCounter IOReadBytes { get; private set; }

        /// <summary>
        /// Gets the IO read operations/sec.
        /// </summary>
        public PerformanceCounter IOReadOperation { get; private set; }

        /// <summary>
        /// Gets the IO Data Bytes/sec.
        /// </summary>
        public PerformanceCounter IODataBytes { get; private set; }

        /// <summary>
        /// Gets the IO Data Operations/sec.
        /// </summary>
        public PerformanceCounter IODataOperation { get; private set; }

        /// <summary>
        /// Gets the elapsed time.
        /// </summary>
        public PerformanceCounter ElapsedTime { get; private set; }

        /// <summary>
        /// The next.
        /// </summary>
        public void Next()
        {
            if (this.ProcessInstance != null)
            {
                this.ProcessorTime.NextValue();
                this.IDProcess.NextValue();
                this.CreatingProcessId.NextValue();
                this.IOWriteBytes.NextValue();
                this.IOWriteOperation.NextValue();
                this.IOReadBytes.NextValue();
                this.IOReadOperation.NextValue();
                this.IODataBytes.NextValue();
                this.IODataOperation.NextValue();
                this.ElapsedTime.NextValue();
            }
        }

        /// <summary>
        /// The print performance counters.
        /// </summary>
        public void PrintPerformanceData() 
        {
            Console.WriteLine("% Processor Time: {0}", this.ProcessorTime.NextValue());
            Console.WriteLine("ID Process: {0}", this.IDProcess.NextValue());
            Console.WriteLine("Creating Process ID: {0}", this.CreatingProcessId.NextValue());
            Console.WriteLine("IO Write Bytes/sec: {0}", this.IOWriteBytes.NextValue());
            Console.WriteLine("IO Write Operations/sec: {0}", this.IOWriteOperation.NextValue());
            Console.WriteLine("IO Read Bytes/sec: {0}", this.IOReadBytes.NextValue());
            Console.WriteLine("IO Read Operations/sec: {0}", this.IOReadOperation.NextValue());
            Console.WriteLine("IO Data Bytes/sec: {0}", this.IODataBytes.NextValue());
            Console.WriteLine("IO Data Operations/sec: {0}", this.IODataOperation.NextValue());
            Console.WriteLine("Elapsed Time: {0}", this.ElapsedTime.NextValue());
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.ProcessorTime.Dispose();
            this.IDProcess.Dispose();
            this.CreatingProcessId.Dispose();
            this.IOWriteBytes.Dispose();
            this.IOWriteOperation.Dispose();
            this.IOReadBytes.Dispose();
            this.IOReadOperation.Dispose();
            this.IODataBytes.Dispose();
            this.IODataOperation.Dispose();
            this.ElapsedTime.Dispose();
        }

        /// <summary>
        /// The init performance counters.
        /// </summary>
        private void InitPerformanceData()
        {
            if (this.ProcessInstance != null)
            {
                this.ProcessorTime = new PerformanceCounter("Process", "% Processor Time", this.ProcessInstance.ProcessName);
                this.IDProcess = new PerformanceCounter("Process", "ID Process", this.ProcessInstance.ProcessName);
                this.CreatingProcessId = new PerformanceCounter("Process", "Creating Process ID", this.ProcessInstance.ProcessName);
                this.IOWriteBytes = new PerformanceCounter("Process", "IO Write Bytes/sec", this.ProcessInstance.ProcessName);
                this.IOWriteOperation = new PerformanceCounter("Process", "IO Write Operations/sec", this.ProcessInstance.ProcessName);
                this.IOReadBytes = new PerformanceCounter("Process", "IO Read Bytes/sec", this.ProcessInstance.ProcessName);
                this.IOReadOperation = new PerformanceCounter("Process", "IO Read Operations/sec", this.ProcessInstance.ProcessName);
                this.IODataBytes = new PerformanceCounter("Process", "IO Data Bytes/sec", this.ProcessInstance.ProcessName);
                this.IODataOperation = new PerformanceCounter("Process", "IO Data Operations/sec", this.ProcessInstance.ProcessName);
                this.ElapsedTime = new PerformanceCounter("Process", "Elapsed Time", this.ProcessInstance.ProcessName);
            }
        }
    }
}
