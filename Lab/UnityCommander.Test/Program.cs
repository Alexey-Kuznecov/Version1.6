
namespace UnityCommander.Test
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using Microsoft.Win32.SafeHandles;

    using UnityCommander.WinDepends;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            string processName = "explorer";

            // foreach (var filePath in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            // {
            //     FileInfo info = new FileInfo(filePath);
            //     total += info.Length;
            // }
            // total = ConverterBytes.AutoConvertBytes(total);
            Process[] processes = Process.GetProcessesByName(processName);
            CopyWatch watch = new CopyWatch(processes[0]);

            while (true)
            {
                watch.WatchProcess();
            }

            // HardDrivePerf hardDrive = new HardDrivePerf();
            //long iODataOperation = 0;
            //long IODataBytes = 0;
            //long iOReadOperation = 0;
            //long iOReadBytes = 0;
            //long iOWriteOperation = 0;
            //long iOWriteBytes = 0;

                //iODataBytes += (long)counter.IODataBytes.NextValue();

                //iOReadOperation += (long)counter.IODataBytes.NextValue();
                //iOReadBytes += (long)counter.IOReadOperation.NextValue();

                //iOWriteOperation += (long)counter.IOWriteOperation.NextValue();
                //iOWriteBytes += (long)counter.IOWriteBytes.NextValue();

                //Console.Write(
                //    "IODataBytes: {0}\n IODataOperation: {1}\n  IOWriteBytes: {2}\n  IOWriteOperation: {3}\n IOReadBytes: {4}\n IOReadOperation: {0}\n",
                //    ConverterBytes.AutoConvertFormatBytes(iODataBytes),
                //    ConverterBytes.AutoConvertFormatBytes(iODataOperation),
                //    ConverterBytes.AutoConvertFormatBytes(iOWriteBytes),
                //    ConverterBytes.AutoConvertFormatBytes(iOWriteOperation),
                //    ConverterBytes.AutoConvertFormatBytes(iOReadBytes),
                //    ConverterBytes.AutoConvertFormatBytes(iOReadOperation));
            //Console.WriteLine();
            //Console.WriteLine(new string('–', 50));
            //hardDrive.PrintPerformanceData();

            Console.WriteLine();
            Console.ReadKey();
        }

        static public class FileUtil
        {
            [StructLayout(LayoutKind.Sequential)]
            struct RM_UNIQUE_PROCESS
            {
                public int dwProcessId;
                public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
            }

            const int RmRebootReasonNone = 0;
            const int CCH_RM_MAX_APP_NAME = 255;
            const int CCH_RM_MAX_SVC_NAME = 63;

            enum RM_APP_TYPE
            {
                RmUnknownApp = 0,
                RmMainWindow = 1,
                RmOtherWindow = 2,
                RmService = 3,
                RmExplorer = 4,
                RmConsole = 5,
                RmCritical = 1000
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            struct RM_PROCESS_INFO
            {
                public RM_UNIQUE_PROCESS Process;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_APP_NAME + 1)]
                public string strAppName;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_SVC_NAME + 1)]
                public string strServiceShortName;

                public RM_APP_TYPE ApplicationType;
                public uint AppStatus;
                public uint TSSessionId;
                [MarshalAs(UnmanagedType.Bool)]
                public bool bRestartable;
            }

            [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
            static extern int RmRegisterResources(uint pSessionHandle,
                                                  UInt32 nFiles,
                                                  string[] rgsFilenames,
                                                  UInt32 nApplications,
                                                  [In] RM_UNIQUE_PROCESS[] rgApplications,
                                                  UInt32 nServices,
                                                  string[] rgsServiceNames);

            [DllImport("rstrtmgr.dll", CharSet = CharSet.Auto)]
            static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

            [DllImport("rstrtmgr.dll")]
            static extern int RmEndSession(uint pSessionHandle);

            [DllImport("rstrtmgr.dll")]
            static extern int RmGetList(uint dwSessionHandle,
                                        out uint pnProcInfoNeeded,
                                        ref uint pnProcInfo,
                                        [In, Out] RM_PROCESS_INFO[] rgAffectedApps,
                                        ref uint lpdwRebootReasons);

            /// <summary>
            /// Find out what process(es) have a lock on the specified file.
            /// </summary>
            /// <param name="path">Path of the file.</param>
            /// <returns>Processes locking the file</returns>
            /// <remarks>See also:
            /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa373661(v=vs.85).aspx
            /// http://wyupdate.googlecode.com/svn-history/r401/trunk/frmFilesInUse.cs (no copyright in code at time of viewing)
            /// 
            /// </remarks>
            static public List<Process> WhoIsLocking(string path)
            {
                uint handle;
                string key = Guid.NewGuid().ToString();
                List<Process> processes = new List<Process>();

                int res = RmStartSession(out handle, 0, key);
                if (res != 0) throw new Exception("Could not begin restart session.  Unable to determine file locker.");

                try
                {
                    const int ERROR_MORE_DATA = 234;
                    uint pnProcInfoNeeded = 0,
                         pnProcInfo = 0,
                         lpdwRebootReasons = RmRebootReasonNone;

                    string[] resources = new string[] { path }; // Just checking on one resource.

                    res = RmRegisterResources(handle, (uint)resources.Length, resources, 0, null, 0, null);

                    if (res != 0) throw new Exception("Could not register resource.");

                    //Note: there's a race condition here -- the first call to RmGetList() returns
                    //      the total number of process. However, when we call RmGetList() again to get
                    //      the actual processes this number may have increased.
                    res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, null, ref lpdwRebootReasons);

                    if (res == ERROR_MORE_DATA)
                    {
                        // Create an array to store the process results
                        RM_PROCESS_INFO[] processInfo = new RM_PROCESS_INFO[pnProcInfoNeeded];
                        pnProcInfo = pnProcInfoNeeded;

                        // Get the list
                        res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, processInfo, ref lpdwRebootReasons);
                        if (res == 0)
                        {
                            processes = new List<Process>((int)pnProcInfo);

                            // Enumerate all of the results and add them to the 
                            // list to be returned
                            for (int i = 0; i < pnProcInfo; i++)
                            {
                                try
                                {
                                    processes.Add(Process.GetProcessById(processInfo[i].Process.dwProcessId));
                                }
                                // catch the error -- in case the process is no longer running
                                catch (ArgumentException) { }
                            }
                        }
                        else throw new Exception("Could not list processes locking resource.");
                    }
                    else if (res != 0) throw new Exception("Could not list processes locking resource. Failed to get size of result.");
                }
                finally
                {
                    RmEndSession(handle);
                }

                return processes;
            }
        }
    }
}