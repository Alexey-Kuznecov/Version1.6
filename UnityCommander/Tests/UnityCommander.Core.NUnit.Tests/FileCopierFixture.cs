
#define Nlog

namespace UnityCommander.Core.NUnit.Tests
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using global::NUnit.Framework;
    using global::System.Collections.Generic;

    using NLog;

    using UnityCommander.Core.IO;

    /// <summary>
    /// The file copier fixture.
    /// </summary>
    [TestFixture]
    public class FileCopierFixture
    {
        #region Declaration Fields

#if (Nlog)

        /// <summary>
        /// The log manager.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
#endif

        /// <summary>
        /// The <c>io</c> read snapshot.
        /// </summary>
        private static readonly List<double> IOReadSnapshot = new List<double>();

        /// <summary>
        /// The io read snapshot.
        /// </summary>
        private static readonly Queue<long> AvIOReadSnapshot = new Queue<long>(30);

        /// <summary>
        /// The snapshot.
        /// </summary>
        private static readonly List<double> Snapshot = new List<double>();

        /// <summary>
        /// The counter.
        /// </summary>
        private static readonly PerformanceCounter Counter = new PerformanceCounter("Process", "IO Read Bytes/sec", "testhost");

        /// <summary>
        /// The manager.
        /// </summary>
        private static readonly FileCopierManager CopyCommand = (FileCopierManager)Commander<FileCopier>.GetManager();

        /// <summary>
        /// The total performance bytes.
        /// </summary>
        private static long totalPerfBytes = 0;

        /// <summary>
        /// The total bytes.
        /// </summary>
        private static long totalBytes = 0;

        /// <summary>
        /// The parameters.
        /// </summary>
        private static FileCopier.Parameters parameters;

        /// <summary>
        /// The file copier.
        /// </summary>
        private FileCopier fileCopier;

        /// <summary>
        /// The source.
        /// </summary>
        private string source;

        /// <summary>
        /// The target.
        /// </summary>
        private string target;

        /// <summary>
        /// The start performance test.
        /// </summary>
        private bool startPerfTest = true;

        #endregion

        /// <summary>
        /// This method is created.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.source = "G:\\Works\\UnitTests\\Source\\Big";
            this.target = "G:\\Works\\UnitTests\\Target";

            this.MeasureSpeed();
        }

        /// <summary>
        /// Testing the result of the <see cref="FileCopier"/> class. Checks an average speed
        /// of copying files per second it should equal the <c>true</c> speed.
        /// </summary>
        [Test]
        public void FileCopier_SpeedAdjustment_EqualTrueSpeed()
        {
            var count = 0;

            foreach (var readByte in IOReadSnapshot)
            {
                var compareTo = Snapshot[count].CompareTo(readByte);
                count++;
            }  
        }

        /// <summary>
        /// The measure speed.
        /// </summary>
        public void MeasureSpeed()
        {
            Task.Factory.StartNew(
                () =>
            {
                while (startPerfTest)
                {
                    var readBytes = Counter.NextValue();
                    totalPerfBytes += (long)Math.Round(readBytes);
                    if (AvIOReadSnapshot.Count > 30)
                    {
                        AvIOReadSnapshot.Dequeue();
                    }

                    AvIOReadSnapshot.Enqueue((long)readBytes);

                    if (readBytes > 0)
                    {
                        var avarageSpeed = AvIOReadSnapshot.Average();
                        IOReadSnapshot.Add(avarageSpeed);
                    }
                }
            });
        }

        /// <summary>
        /// Testing the result of the <see cref="FileCopier"/> class. Checks the progress of copying files.
        /// The percentage must be greater or equal to 0 and less or equal to 100.
        /// </summary>
        [Test]
        public void FileCopier_PercentAdjustment_EqualFrom0To100()
        {
            using (this.fileCopier = new FileCopier())
            {
                // Average
                this.fileCopier.CopyProgressReport += this.FileCopier_CopyProgressReport;
                this.fileCopier.CalculateTotalFilesSize(this.source);
                //this.fileCopier.GetSpeedTimer.Start();
                this.fileCopier.GetElapsedTimer.Start();

                // Act
                foreach (var oldDir in Directory.GetDirectories(this.source, "*", SearchOption.AllDirectories))
                {
                    var newDir = oldDir.Replace(this.source, this.target);
                    Directory.CreateDirectory(newDir);
                    this.fileCopier.CopyFiles(oldDir, newDir);
                }

                // Also copy files in the root directory of the source.
                if (Directory.GetFiles(this.source).Length != 0)
                {
                    this.fileCopier.CopyFiles(this.source, this.target);
                }
            }

            // Assert
            Directory.Delete(this.target, true);
            this.startPerfTest = false;
            //Assert.AreEqual(parameters.TotalTimeLeft, TimeSpan.Zero);
            //Assert.That(parameters.TotalPercentage == 100);
        }

        /// <summary>
        /// Testing the result of the <see cref="FileCopier"/> class. Checks how much time elapsed
        /// after the files have finished copying to the destination. The time must be no more than 0.
        /// </summary>
        public void FileCopier_TimeLeftAdjustment_EqualTo0()
        {
            Assert.Equals(parameters.TotalTimeLeft, TimeSpan.Zero);
        }
        
        /// <summary>
        /// The copy command copy file report.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> Expected report data about copy file. </param>
        private void FileCopier_CopyProgressReport(object sender, FileCopier.Parameters e)
        {
            parameters = e;
            totalBytes += (long)Math.Round(parameters.AverageSpeed);
            Snapshot.Add(parameters.AverageSpeed);
            Assert.That(parameters.TotalPercentage >= 0 && parameters.TotalPercentage <= 100);
        }
    }
}
