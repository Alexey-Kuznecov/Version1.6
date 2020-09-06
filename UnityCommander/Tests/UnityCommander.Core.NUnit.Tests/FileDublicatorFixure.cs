#define Nlog
using System;
using System.IO;
using System.Security.Cryptography;
using NLog;
using NUnit.Framework;
using UnityCommander.Core.IO;

namespace UnityCommander.Core.NUnit.Tests
{
    [TestFixture]
    public class Tests
    {
        private FileDublicator _dublicator;
        private int _currentStep;
        private string _currentFileSource;
        private DirectoryInfo _source;
        private DirectoryInfo _target;
        /// <summary>
        /// The Logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [SetUp]
        public void Setup()
        {
            this._source = new DirectoryInfo("h:\\Books");
            this._target = new DirectoryInfo("h:\\Works\\UnitTests\\Target");
            this._dublicator = new FileDublicator();
            FileDublicator.CopyingEvent += FileDublicator_CopyingEvent;
        }
        /// <summary>
        /// Send a report on the correct step of copying the file.
        /// Testing the method <see cref="FileDublicator.CopyFileByte(string, string)"/>
        /// </summary>
        [Test, Order(1)]
        public void CopyFileByte_SendReportOnCorrectStepCopyFile_StepFrom0To100()
        {
            foreach (string dirPath in Directory.GetDirectories(this._source.FullName, "*", SearchOption.AllDirectories))
            {
                var newDir = dirPath.Replace(this._source.FullName, this._target.FullName);

                Directory.CreateDirectory(newDir);

                foreach (var oldFile in Directory.GetFiles(dirPath))
                {
                    this._currentFileSource = oldFile;
                    string currentFileTarget = Path.Combine(newDir, new DirectoryInfo(oldFile).Name);
                    this._dublicator.CopyFileByte(oldFile, currentFileTarget);
                }
            }
            // NUnit testing
            Assert.That(this._currentStep >= 0 && this._currentStep <= 100);
        }
        /// <summary>
        /// Compares the total number of source and target directories.
        /// </summary>
        [Test, Order(2)]
        public void CopyFileByte_CalcDirs()
        {
            int sTotalNumDir = CalcDirectory(this._source.FullName);
            int tTotalNumDir = CalcDirectory(this._target.FullName);           
            // Total number directory
            Assert.That(sTotalNumDir == tTotalNumDir);
        }
        /// <summary>
        /// Compares the total number of source and target files.
        /// </summary>
        [Test, Order(3)]
        public void CopyFileByte_CalcFiles()
        {
            int sTotalNumFile = CalcFile(this._source.FullName);
            int tTotalNumFile = CalcFile(this._target.FullName);
            // Total number directory
            Assert.That(sTotalNumFile == tTotalNumFile);
        }
        /// <summary>
        /// Checks if a file exists and its integrity by comparing source and target file.
        /// </summary>
        [Test, Order(4)]
        public void CopyFileByte_FullFileScan()
        {
            bool control—heck = FullFileScan();
            // Checks if file exist in the destination and also checks file control summ.
            Assert.IsTrue(control—heck);
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
            this._currentStep = e.ProgressBarInfo.ProgressBar;

            if (this._currentStep >= 99)
            {
                FileInfo info = new FileInfo(this._currentFileSource);
#if (Nlog1)
                Logger.Info("Name {0}; Length {1}; Path {2}; Load {3}%", info.Name, info.Length, info.FullName, this._currentStep);
#endif        
            }
        }
        /// <summary>
        /// Calculates all subdirectory at the directory.
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
                if (!item.Contains("desktop.ini"))
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
            DirectoryInfo info = new DirectoryInfo(this._source.FullName);
            foreach (var soureFile in Directory.GetFiles(info.FullName, "*", SearchOption.AllDirectories))
            {
                if (soureFile.Contains("desktop.ini"))
                    continue;

                string destFile = soureFile.Replace(this._source.FullName, this._target.FullName);           
                // To check if file exist in the destination 
                if (File.Exists(destFile))
                {
                    // Checking the integrity of a file at the destination after copying files from the source.
                    string tCheckSum = this.CheckSum(destFile);
                    string sCheckSum = this.CheckSum(soureFile);
#if (Nlog)
                    string fileName = destFile.Substring(destFile.LastIndexOf('\\')).Substring(1);
                    Logger.Info("'{2}:' {0} => {1}", tCheckSum, sCheckSum, fileName);
#endif          
                    if (tCheckSum != sCheckSum)
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