using NUnit.Framework;
using System.IO;
using UnityCommander.Core.IO;

namespace UnityCommander.Core.NUnit.Tests
{
    public class Tests
    {
        private int _currentStep;

        [SetUp]
        public void Setup()
        {

        }
        /// <summary>
        /// Send a report on the correct step of copying the file.
        /// </summary>
        [Test]
        public void CopyFileByte_SendReportOnCorrectStepCopyFile_StepFrom0To100()
        {
            FileDublicator dublicator = new FileDublicator();
            dublicator.CopyFileByte("h:\\Works\\UnitTests\\Source\\", "h:\\Works\\UnitTests\\Target\\");
            FileDublicator.CopyingEvent += FileDublicator_CopyingEvent;
            Assert.That(this._currentStep >= 0 && this._currentStep <= 100);
        }

        private void FileDublicator_CopyingEvent(object sender, CopyInfoEventArgs e)
        {
            this._currentStep = e.ProgressBarInfo.ProgressBar;
        }
    }
}