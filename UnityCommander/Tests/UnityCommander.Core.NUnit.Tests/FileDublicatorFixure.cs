using NUnit.Framework;
using System.Diagnostics;
using System.IO;
using UnityCommander.Core.IO;

namespace UnityCommander.Core.NUnit.Tests
{
    public class Tests
    {
        private int _currentStep;
        private string _source;
        private string _target;

        [SetUp]
        public void Setup()
        {
            DirectoryInfo src = new DirectoryInfo("h:\\Books");
            DirectoryInfo dest = new DirectoryInfo("h:\\Works\\UnitTests\\Target");

            foreach (string dirPath in Directory.GetDirectories(src.FullName, "*",
                    SearchOption.AllDirectories))
            {
                var newDir = dirPath.Replace(src.FullName, dest.FullName);

                Directory.CreateDirectory(newDir);

                foreach (var oldDir in Directory.GetFiles(dirPath))
                {
                    _source = oldDir;
                    _target = Path.Combine(newDir, new DirectoryInfo(oldDir).Name);
                    CopyFileByte_SendReportOnCorrectStepCopyFile_StepFrom0To100();
                }
            }
        }

        /// <summary>
        /// Send a report on the correct step of copying the file.
        /// </summary>
        [Test]
        public void CopyFileByte_SendReportOnCorrectStepCopyFile_StepFrom0To100()
        {
            FileDublicator dublicator = new FileDublicator();
            dublicator.CopyFileByte(_source, _target);
            FileDublicator.CopyingEvent += FileDublicator_CopyingEvent;
            Assert.That(this._currentStep >= 0 && this._currentStep <= 102);
        }

        private void FileDublicator_CopyingEvent(object sender, CopyInfoEventArgs e)
        {
            Trace.WriteLine(e.ProgressBarInfo.ErrorMessage);
            this._currentStep = e.ProgressBarInfo.ProgressBar;
        }
    }
}