
namespace UnityCommander.Test
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// The flash performance form.
    /// </summary>
    public class FlashPerformanceForm
    {
        /// <summary>
        /// The test drive letter.
        /// </summary>
        private string testDriveLetter;

        /// <summary>
        /// The test file size.
        /// </summary>
        private int testFileSize;

        /// <summary>
        /// The test iterations.
        /// </summary>
        private int testIterations;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlashPerformanceForm"/> class.
        /// </summary>
        public FlashPerformanceForm()
        {
            this.testDriveLetter = "H:";
            this.testFileSize = 200 * 1000000;
            this.testIterations = 5;
            Thread testThread = new Thread(new ThreadStart(this.TestPerf));
            testThread.Start();
        }

        /// <summary>
        /// The test perf.
        /// </summary>
        private void TestPerf()
        {
            try
            {
                int appendIterations = this.testFileSize / 100000;
                
                string runText = "Running a " + this.testFileSize / 1000000 + "MB file write on drive "
                    + this.testDriveLetter + " " + this.testIterations + " times...";
                Console.WriteLine(runText);
                
                double totalPerf = 0;

                string randomText = this.RandomString(100000);

                for (int j = 1; j <= this.testIterations; j++)
                {
                    if (File.Exists(System.Environment.CurrentDirectory + "\\" +
                                    j + "test.tmp")) 
                    {
                        File.Delete(System.Environment.CurrentDirectory + "\\" +
                                    j + "test.tmp");
                    }
                    
                    StreamWriter streamWriter = new StreamWriter(System.Environment.CurrentDirectory + "\\" + j + "test.tmp", true);

                    for (int i = 1; i <= appendIterations; i++)
                    {
                        streamWriter.Write(randomText);
                    }

                    streamWriter.Close();
                    var startTime = DateTime.Now;
                    
                    File.Copy(System.Environment.CurrentDirectory + "\\" + j + "test.tmp", this.testDriveLetter + "\\" + j + "test.tmp");  
                    var stopTime = DateTime.Now;
                    File.Delete(System.Environment.CurrentDirectory + "\\" + j + "test.tmp");
                    File.Delete(this.testDriveLetter + "\\" + j + "test.tmp");
                    TimeSpan interval = stopTime - startTime;

                    string result = "Iteration " + j + ":   " + Math.Round((this.testFileSize / 1000) / interval.TotalMilliseconds,
                                        2) + " MB/sec";

                    Console.WriteLine(result);
                    totalPerf += (this.testFileSize / 1000) / interval.TotalMilliseconds;
                }

                Console.WriteLine("------------------------------");
                Console.WriteLine("Average:       " + Math.Round(totalPerf / this.testIterations, 2) + " MB/sec");
                Console.WriteLine("------------------------------");
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured: " + e.Message);
            }
        }

        /// <summary>
        /// The random string.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));                 
                builder.Append(ch);
            }

            return builder.ToString();
        }

        /// <summary>
        /// The get removable drive letters.
        /// </summary>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        private string[] GetRemovableDriveLetters()
        {
            System.Collections.ArrayList RemovableDriveLetters = new System.Collections.ArrayList();
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.DriveType == DriveType.Removable)
                {
                    RemovableDriveLetters.Add(d.Name.Substring(0, 1));
                }
            }

            return RemovableDriveLetters.ToArray(typeof(string)) as string[];
        }
    }
}
