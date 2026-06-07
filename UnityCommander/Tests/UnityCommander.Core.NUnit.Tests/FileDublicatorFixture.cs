
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileDublicatorFixture.cs" company="T">
// Copyright (p) Alexey Kuznecov. All right reserved.
// </copyright>
// <summary>
//  The class for testing methods of the FileDublicator class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#define Nlog

namespace UnityCommander.Core.NUnit.Tests 
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using NLog;
    using global::NUnit.Framework;
    using UnityCommander.Core.IO;

    /// <summary>
    /// The class for testing methods of the <see cref="FileDublicator"/> class.
    /// </summary>
    [TestFixture]
    public class FileDublicatorFixture
    {
        #region Declaration fields

        /// <summary>
        /// The Logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The instance of the <see cref="FileDublicator"/> class.
        /// </summary>
        //private FileDublicator dublicator;

        /// <summary>
        /// The current step of the copying file.
        /// </summary>
        private int currentStep;

        /// <summary>
        /// The current path file source.
        /// </summary>
        private string currentFileSource;

        /// <summary>
        /// The source file information.
        /// </summary>
        private DirectoryInfo source;

        /// <summary>
        /// The information about the file at the destination.
        /// </summary>
        private DirectoryInfo target;

        #endregion

        /// <summary>
        /// Initializes variables and events.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            //this.source = new DirectoryInfo("h:\\Works\\UnitTests\\Source");
            //this.target = new DirectoryInfo("h:\\Works\\UnitTests\\Target");
            //this.dublicator = new FileDublicator();
            //FileDublicator.CopyingEvent += this.FileDublicator_CopyingEvent;
        }

        /// <summary>
        /// Send a report on the correct step of copying the file.
        /// Testing the method <see cref="FileDublicator.CopyFileByte(string, string)"/>
        /// </summary>
        [Test, Order(1)]
        public void CopyFileByte_SendReportOnCorrectStepCopyFile_StepFrom0To100()
        {
            foreach (string dirPath in Directory.GetDirectories(this.source.FullName, "*", SearchOption.AllDirectories))
            {
                var newDir = dirPath.Replace(this.source.FullName, this.target.FullName);

                Directory.CreateDirectory(newDir);

                foreach (var oldFile in Directory.GetFiles(dirPath))
                {
                    this.currentFileSource = oldFile;
                    string currentFileTarget = Path.Combine(newDir, new DirectoryInfo(oldFile).Name);
                    //this.dublicator.CopyFileByte(oldFile, currentFileTarget);
                }
            }

            // NUnit testing
            Assert.That(this.currentStep >= 0 && this.currentStep <= 100);
        }

        /// <summary>
        /// Compares the total number of source and target directories.
        /// </summary>
        [Test, Order(2)]
        public void CopyFileByte_CalcDirs()
        {
            int sourceCalcFile = this.CalcDirectory(this.source.FullName);
            int targetCalcFile = this.CalcDirectory(this.target.FullName);

            // Total number directory
            Assert.That(sourceCalcFile == targetCalcFile);
        }

        /// <summary>
        /// Compares the total number of source and target files.
        /// </summary>
        [Test, Order(3)]
        public void CopyFileByte_CalcFiles()
        {
            int sourceCalcFile = this.CalcFile(this.source.FullName);
            int targetCalcFile = this.CalcFile(this.target.FullName);

            // Total number directory
            Assert.That(sourceCalcFile == targetCalcFile);
        }

        /// <summary>
        /// Checks if a file exists and its integrity by comparing source and target file.
        /// </summary>
        [Test, Order(4)]
        public void CopyFileByte_FullFileScan()
        {
            bool controlCheck = this.FullFileScan();

            // Checks if file exist in the destination and also checks file control sum.
            Assert.IsTrue(controlCheck);
        }

        #region Helper methods

        /// <summary>
        /// The event handler is needed to get the copy progress from the 
        /// <see cref="FileDublicator.CopyFileByte(string, string)"/> method.
        /// </summary>
        /// <param name="sender"> The event initiator class. </param>
        /// <param name="e"> Expected <see cref="CopyInfoEventArgs.ProgressBarInfo"/> property of the <see cref="CopyInfoEventArgs"/> object. </param>
        private void FileDublicator_CopyingEvent(object sender, CopyInfoEventArgs e)
        {
            this.currentStep = e.ProgressBarInfo.ProgressBar;

            if (this.currentStep >= 99)
            {
                FileInfo info = new FileInfo(this.currentFileSource);
#if (Nlog1)
                    Logger.Info("Name {0}; Length {1}; Path {2}; Load {3}%", info.Name, info.Length, info.FullName, this.currentStep);
#endif
            }
        }

        /// <summary>
        /// Calculates all subdirectories at the directory.
        /// </summary>
        /// <param name="path"> Path to directory. </param>
        /// <returns> Total number of subdirectories found. </returns>
        private int CalcDirectory(string path)
        {
            int count = 0;
            DirectoryInfo info = new DirectoryInfo(path);
            foreach (var item in Directory.GetDirectories(info.FullName, "*", SearchOption.AllDirectories))
            {
                count++;
            }

            Logger.Info("Total catalogs: {0}", count);
            return count;
        }

        /// <summary>
        /// Calculates all files found in a directory as well as subdirectories. 
        /// </summary>
        /// <param name="path"> Path to directory. </param>
        /// <returns> Total number of files, found in all directories. </returns>
        private int CalcFile(string path)
        {
            int count = 0;
            DirectoryInfo info = new DirectoryInfo(path);
            foreach (var item in Directory.GetFiles(info.FullName, "*", SearchOption.AllDirectories))
            {
                count++;
            }

            Logger.Info("Total files: {0}", count);
            return count;
        }

        /// <summary>
        /// Checks if file exist in the destination after files copying and also
        /// checks integrity of the file in the destination,
        /// </summary>
        /// <returns> True if the target file is exactly the same as the source file. </returns>
        private bool FullFileScan()
        {
            DirectoryInfo info = new DirectoryInfo(this.source.FullName);
            foreach (var sourceFile in Directory.GetFiles(info.FullName, "*", SearchOption.AllDirectories))
            {
                string destFile = sourceFile.Replace(this.source.FullName, this.target.FullName);

                // To check if file exist in the destination 
                if (File.Exists(destFile))
                {
                    // Checking the integrity of a file at the destination after copying files from the source.
                    string targetCheckSum = this.CheckSum(destFile);
                    string sourceCheckSum = this.CheckSum(sourceFile);
#if (Nlog)
                    string fileName = destFile.Substring(destFile.LastIndexOf('\\')).Substring(1);
                    Logger.Info("'{2}:' {0} => {1}", targetCheckSum, sourceCheckSum, fileName);
#endif
                    if (targetCheckSum != sourceCheckSum)
                        return false;
                }
                else
                {
#if (Nlog)
                    Logger.Error("File is not exist: {0}", destFile);
#endif
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Calculate MD5 checksum for a file.
        /// </summary>
        /// <param name="path"> The path to the file. </param>
        /// <returns> Checksum of the file. </returns>
        private string CheckSum(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        #endregion
    }
}