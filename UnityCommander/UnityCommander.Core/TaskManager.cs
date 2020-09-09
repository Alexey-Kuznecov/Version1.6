
namespace UnityCommander.Core
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The task manager.
    /// </summary>
    public class TaskManager
    {
        /// <summary>
        /// The thread access.
        /// </summary>
        [Flags]
        public enum ThreadAccess : int
        {
            /// <summary>
            /// The terminate.
            /// </summary>
            TERMINATE = (0x0001),

            /// <summary>
            /// The suspend resume.
            /// </summary>
            SUSPEND_RESUME = (0x0002),

            /// <summary>
            /// The get context.
            /// </summary>
            GET_CONTEXT = (0x0008),

            /// <summary>
            /// The set context.
            /// </summary>
            SET_CONTEXT = (0x0010),

            /// <summary>
            /// The set information.
            /// </summary>
            SET_INFORMATION = (0x0020),

            /// <summary>
            /// The query information.
            /// </summary>
            QUERY_INFORMATION = (0x0040),

            /// <summary>
            /// The set thread token.
            /// </summary>
            SET_THREAD_TOKEN = (0x0080),

            /// <summary>
            /// The impersonate.
            /// </summary>
            IMPERSONATE = (0x0100),

            /// <summary>
            /// The direct impersonation.
            /// </summary>
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);

        /// <summary>
        /// The suspend process.
        /// </summary>
        /// <param name="pid">
        /// The pid.
        /// </param>
        public static void SuspendProcess(int pid)
        {
            var process = Process.GetProcessById(pid); // throws exception if process does not exist

            foreach (ProcessThread pT in process.Threads)
            {
                var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                SuspendThread(pOpenThread);

                CloseHandle(pOpenThread);
            }
        }

        /// <summary>
        /// The resume process.
        /// </summary>
        /// <param name="pid">
        /// The pid.
        /// </param>
        public static void ResumeProcess(int pid)
        {
            var process = Process.GetProcessById(pid);

            if (process.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                var suspendCount = 0;
                do
                {
                    suspendCount = ResumeThread(pOpenThread);
                } while (suspendCount > 0);

                CloseHandle(pOpenThread);
            }
        }
    }
}