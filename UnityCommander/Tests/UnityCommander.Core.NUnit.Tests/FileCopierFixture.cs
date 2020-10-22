
#define Nlog

namespace UnityCommander.Core.NUnit.Tests
{
    using System;
    using System.Globalization;
    using System.IO;
    using global::NUnit.Framework;
    using NLog;

    using UnityCommander.Core.Helper;
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
        /// The current step.
        /// </summary>
        private double currentStep;

        /// <summary>
        /// The source.
        /// </summary>
        private string source;

        /// <summary>
        /// The target.
        /// </summary>
        private string target;


        #endregion

        /// <summary>
        /// This method is created.
        /// </summary>
        [SetUp]
        public void Setup()
        {
#if (Nlog)
            LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("../../../NLog.config");
            Logger.Info("Starting Testing of the FileCopier class.");
#endif

            this.source = "e:\\Works\\UnitTests\\Source\\Big";
            this.target = "e:\\Works\\UnitTests\\Target";
            
            FileCopier.CopyProgressReport += this.FileCopier_OnCopyProgressReport;
            FileCopier.InitialDataTest();
        }

        /// <summary>   
        /// This method is created.
        /// </summary>
        [Test]
        public void FileCopier_ProgressPercentage_From0To100()
        {
            FileCopier.MeasureTotalFilesSize(this.source);
            FileCopier.GetTotalTimer.Start();

            foreach (var oldDir in Directory.GetDirectories(this.source, "*", SearchOption.AllDirectories))
            {
                var newDir = oldDir.Replace(this.source, this.target);
                Directory.CreateDirectory(newDir);
                FileCopier.CopyFiles(oldDir, newDir);
                this.CopyFileResult(FileCopier.CopyInfoInstance);
                // Logger.Info(LogFormatter.DrawObject(FileCopier.CopyInfoInstance));
            }

            if (Directory.GetFiles(this.source).Length != 0)
            {
                FileCopier.CopyFiles(this.source, this.target);
            }

            FileCopier.GetTotalTimer.Start();
        }

        /// <summary>
        /// The file copier_ on copy progress report.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The e. </param>
        private void FileCopier_OnCopyProgressReport(object sender, FileCopier.CopyInfo e)
        {
            decimal averageSpeed = Math.Round(ConverterBytes.AutoConvertBytes((decimal)e.AverageSpeed), 1);
            Logger.Info(LogFormatter.DrawHeader(e.Percentage.ToString(CultureInfo.InvariantCulture)));
            Logger.Info(LogFormatter.DrawObject(e));
            this.currentStep = e.Percentage;
        }

        /// <summary>
        /// The file copier copy file result.
        /// </summary>
        /// <param name="e"> The e. </param>
        private void CopyFileResult(FileCopier.CopyInfo e)
        {
            if (e.Percentage > 100)
            {
                throw new ArgumentException();
            }
        }
    }
}
