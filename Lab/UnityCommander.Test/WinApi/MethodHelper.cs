
namespace UnityCommander.Test.WinApi
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;

    using Microsoft.Win32.SafeHandles;

    using NLog;

    /// <summary>
    /// The process utility.
    /// </summary>
    public class MethodHelper
    {
        /// <summary>
        /// The max path.
        /// </summary>
        private const int MaxPath = 260;

        /// <summary>
        /// The handle type token count.
        /// </summary>
        private const int HandleTypeTokenCount = 27;

        /// <summary>
        /// The network device prefix.
        /// </summary>
        private const string NetworkDevicePrefix = "\\Device\\LanmanRedirector\\";

        /// <summary>
        /// The handle type tokens.
        /// </summary>
        private static readonly string[] HandleTypeTokens =
            {
                string.Empty, string.Empty, "Directory", "SymbolicLink", "Token", "Process", "Thread", "Unknown7", "Event", "EventPair",
                "Mutant", "Unknown11", "Semaphore", "Timer", "Profile", "WindowStation", "Desktop", "Section", "Key",
                "Port", "WaitablePort", "Unknown21", "Unknown22", "Unknown23", "Unknown24", "IoCompletion", "File"
            };

        /// <summary>
        /// The device map.
        /// </summary>
        private static Dictionary<string, string> deviceMap;

        /// <summary>
        /// The int ptr add.
        /// </summary>
        /// <param name="ptr"> The ptr. </param>
        /// <param name="offset"> The offset. </param>
        /// <returns> The <see cref="IntPtr"/>. </returns>
        public static IntPtr IntPtrAdd(IntPtr ptr, int offset)
        {
            if (IntPtr.Size == 4)
            {
                return (IntPtr)((int)ptr + offset);
            }
            else
            {
                return (IntPtr)((long)ptr + offset);
            }
        }


        /// <summary>
        /// The get process id.
        /// </summary>
        /// <param name="processId"> The process id. </param>
        /// <returns> The <see cref="int"/>. </returns>
        public static int GetProcessId(IntPtr processId)
        {
            if (IntPtr.Size == 4)
            {
                return (int)processId;
            }

            return (int)((long)processId >> 32);
        }

        /// <summary>
        /// The get file name from handle.
        /// </summary>
        /// <param name="handle"> The handle. </param>
        /// <param name="processId"> The process id. </param>
        /// <param name="fileName"> The file name. </param>
        /// <returns> The <see cref="bool"/>. </returns>
        public static bool GetFileNameFromHandle(IntPtr handle, int processId, out string fileName)
        {
            IntPtr currentProcess = NativeMethods.GetCurrentProcess();
            bool remote = processId != NativeMethods.GetProcessId(currentProcess);
            SafeProcessHandle processHandle = null;
            SafeObjectHandle objectHandle = null;
            try
            {
                if (remote)
                {
                    processHandle = NativeMethods.OpenProcess(ProcessAccessRights.PROCESS_DUP_HANDLE, true, processId);
                    if (NativeMethods.DuplicateHandle(processHandle.DangerousGetHandle(), handle, currentProcess, out objectHandle, 0, false, DuplicateHandleOptions.DUPLICATE_SAME_ACCESS))
                    {
                        handle = objectHandle.DangerousGetHandle();
                    }
                }

                return GetFileNameFromHandle(handle, out fileName, 200);
            }
            finally
            {
                if (remote)
                {
                    processHandle?.Close();
                    objectHandle?.Close();
                }
            }
        }

        /// <summary>
        /// The convert device path to dos path.
        /// </summary>
        /// <param name="devicePath"> The device path. </param>
        /// <param name="dosPath"> The dos path. </param>
        /// <returns> The <see cref="bool"/>. </returns>
        public static bool ConvertDevicePathToDosPath(string devicePath, out string dosPath)
        {
            EnsureDeviceMap();
            int i = devicePath.Length;
            while (i > 0 && (i = devicePath.LastIndexOf('\\', i - 1)) != -1)
            {
                if (deviceMap.TryGetValue(devicePath.Substring(0, i), out var drive))
                {
                    dosPath = string.Concat(drive, devicePath.Substring(i));
                    return dosPath.Length != 0;
                }
            }

            dosPath = string.Empty;
            return false;
        }

        /// <summary>
        /// The get handle type.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <param name="processId"> The process id. </param>
        /// <param name="handleType"> The handle type. </param>
        /// <returns> The <see cref="bool"/>. </returns>
        internal static bool GetHandleType(IntPtr handle, int processId, out SystemHandleType handleType)
        {
            string token = GetHandleTypeToken(handle, processId);
            return GetHandleTypeFromToken(token, out handleType);
        }

        /// <summary>
        /// The get file name from handle.
        /// </summary>
        /// <param name="handle"> The handle. </param>
        /// <param name="fileName"> The file name. </param>
        /// <param name="wait"> The wait. </param>
        /// <returns> The <see cref="bool"/>. </returns>
        private static bool GetFileNameFromHandle(IntPtr handle, out string fileName, int wait)
        {
            using (FileNameFromHandleState f = new FileNameFromHandleState(handle))
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(GetFileNameFromHandle), f);
                if (f.WaitOne(wait))
                {
                    fileName = f.FileName;
                    return f.RetValue;
                }

                fileName = string.Empty;
                return false;
            }
        }

        /// <summary>
        /// The get file name from handle.
        /// </summary>
        /// <param name="state"> The state. </param>
        private static void GetFileNameFromHandle(object state)
        {
            FileNameFromHandleState s = (FileNameFromHandleState)state;
            s.RetValue = GetFileNameFromHandle(s.Handle, out var fileName);
            s.FileName = fileName;
            s.Set();
        }

        /// <summary>
        /// The get file name from handle.
        /// </summary>
        /// <param name="handle"> The handle. </param>
        /// <param name="fileName"> The file name. </param>
        /// <returns> The <see cref="bool"/>. </returns>
        private static bool GetFileNameFromHandle(IntPtr handle, out string fileName)
        {
            IntPtr ptr = IntPtr.Zero;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                int length = 0x200;  // 512 bytes
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                }
                finally
                {
                    // CER guarantees the assignment of the allocated 
                    // memory address to ptr, if an ansynchronous exception 
                    // occurs.
                    ptr = Marshal.AllocHGlobal(length);
                }

                NT_STATUS ret = NativeMethods.NtQueryObject(handle, OBJECT_INFORMATION_CLASS.ObjectNameInformation, ptr, length, out length);
                if (ret == NT_STATUS.STATUS_BUFFER_OVERFLOW)
                {
                    RuntimeHelpers.PrepareConstrainedRegions();
                    try
                    {
                    }
                    finally
                    {
                        // CER guarantees that the previous allocation is freed,
                        // and that the newly allocated memory address is 
                        // assigned to ptr if an asynchronous exception occurs.
                        Marshal.FreeHGlobal(ptr);
                        ptr = Marshal.AllocHGlobal(length);
                    }

                    ret = NativeMethods.NtQueryObject(handle, OBJECT_INFORMATION_CLASS.ObjectNameInformation, ptr, length, out length);
                }

                if (ret == NT_STATUS.STATUS_SUCCESS)
                {
                    OBJECT_NAME_INFORMATION objNameInfo = (OBJECT_NAME_INFORMATION)Marshal.PtrToStructure(ptr, typeof(OBJECT_NAME_INFORMATION));
                    fileName = objNameInfo.Name.Buffer;
                    return fileName?.Length != 0;
                }
            }
            finally
            {
                // CER guarantees that the allocated memory is freed, 
                // if an asynchronous exception occurs.
                Marshal.FreeHGlobal(ptr);
            }

            fileName = string.Empty;
            return false;
        }

        /// <summary>
        /// The get handle type from token.
        /// </summary>
        /// <param name="token"> The token. </param>
        /// <param name="handleType"> The handle type. </param>
        /// <returns> The <see cref="bool"/>. </returns>
        private static bool GetHandleTypeFromToken(string token, out SystemHandleType handleType)
        {
            for (int i = 1; i < HandleTypeTokenCount; i++)
            {
                if (HandleTypeTokens[i] == token)
                {
                    handleType = (SystemHandleType)i;
                    return true;
                }
            }

            handleType = SystemHandleType.OB_TYPE_UNKNOWN;
            return false;
        }

        /// <summary>
        /// The get handle type token.
        /// </summary>
        /// <param name="handle"> The handle. </param>
        /// <param name="processId"> The process id. </param>
        /// <returns> The <see cref="string"/>. </returns>
        private static string GetHandleTypeToken(IntPtr handle, int processId)
        {
            IntPtr currentProcess = NativeMethods.GetCurrentProcess();
            bool remote = processId != NativeMethods.GetProcessId(currentProcess);
            SafeProcessHandle processHandle = null;
            SafeObjectHandle objectHandle = null;
            try
            {
                if (remote)
                {
                    processHandle = NativeMethods.OpenProcess(ProcessAccessRights.PROCESS_DUP_HANDLE, true, processId);
                    if (NativeMethods.DuplicateHandle(processHandle.DangerousGetHandle(), handle, currentProcess, out objectHandle, 0, false, DuplicateHandleOptions.DUPLICATE_SAME_ACCESS))
                    {
                        handle = objectHandle.DangerousGetHandle();
                    }
                }

                return GetHandleTypeToken(handle);
            }
            finally
            {
                if (remote)
                {
                    processHandle?.Close();
                    objectHandle?.Close();
                }
            }
        }

        /// <summary>
        /// The get handle type token.
        /// </summary>
        /// <param name="handle"> The handle. </param>
        /// <returns> The <see cref="string"/>. </returns>
        private static string GetHandleTypeToken(IntPtr handle)
        {
            NativeMethods.NtQueryObject(handle, OBJECT_INFORMATION_CLASS.ObjectTypeInformation, IntPtr.Zero, 0, out var length);
            IntPtr ptr = IntPtr.Zero;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                }
                finally
                {
                    if (length >= 0)
                    {
                        ptr = Marshal.AllocHGlobal(length);
                    }
                }

                if (NativeMethods.NtQueryObject(handle, OBJECT_INFORMATION_CLASS.ObjectTypeInformation, ptr, length, out length) == NT_STATUS.STATUS_SUCCESS)
                {
                    OBJECT_TYPE_INFORMATION objTypeInfo = (OBJECT_TYPE_INFORMATION)Marshal.PtrToStructure(ptr, typeof(OBJECT_TYPE_INFORMATION));
                    return objTypeInfo.TypeName.Buffer;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return string.Empty;
        }

        /// <summary>
        /// The ensure device map.
        /// </summary>
        private static void EnsureDeviceMap()
        {
            if (deviceMap != null) return;
            Dictionary<string, string> localDeviceMap = BuildDeviceMap();
            Interlocked.CompareExchange<Dictionary<string, string>>(ref deviceMap, localDeviceMap, null);
        }

        /// <summary>
        /// The build device map.
        /// </summary>
        /// <returns> The <see cref="Dictionary"/>. </returns>
        private static Dictionary<string, string> BuildDeviceMap()
        {
            string[] logicalDrives = Environment.GetLogicalDrives();
            Dictionary<string, string> localDeviceMap = new Dictionary<string, string>(logicalDrives.Length);
            StringBuilder targetPath = new StringBuilder(MaxPath);
            foreach (string drive in logicalDrives)
            {
                string deviceName = drive.Substring(0, 2);
                NativeMethods.QueryDosDevice(deviceName, targetPath, MaxPath);
                localDeviceMap.Add(NormalizeDeviceName(targetPath.ToString()), deviceName);
            }

            localDeviceMap.Add(NetworkDevicePrefix.Substring(0, NetworkDevicePrefix.Length - 1), "\\");
            return localDeviceMap;
        }

        /// <summary>
        /// The normalize device name.
        /// </summary>
        /// <param name="deviceName"> The device name. </param>
        /// <returns> The <see cref="string"/>.</returns>
        private static string NormalizeDeviceName(string deviceName)
        {
            if (string.Compare(deviceName, 0, NetworkDevicePrefix, 0, NetworkDevicePrefix.Length, StringComparison.InvariantCulture) == 0)
            {
                string shareName = deviceName.Substring(deviceName.IndexOf('\\', NetworkDevicePrefix.Length) + 1);
                return string.Concat(NetworkDevicePrefix, shareName);
            }

            return deviceName;
        }

        /// <summary>
        /// The safe object handle.
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        internal sealed class SafeObjectHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SafeObjectHandle"/> class.
            /// </summary>
            /// <param name="preexistingHandle"> The preexisting handle. </param>
            /// <param name="ownsHandle"> The owns handle. </param>
            internal SafeObjectHandle(IntPtr preexistingHandle, bool ownsHandle)
                : base(ownsHandle)
            {
                this.SetHandle(preexistingHandle);
            }

            /// <summary>
            /// Prevents a default instance of the <see cref="SafeObjectHandle"/> class from being created.
            /// </summary>
            private SafeObjectHandle()
                : base(true)
            {
            }

            /// <summary>
            /// The release handle.
            /// </summary>
            /// <returns> The <see cref="bool"/>.</returns>
            protected override bool ReleaseHandle()
            {
                return NativeMethods.CloseHandle(this.handle);
            }
        }

        /// <summary>
        /// The safe process handle.
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        internal sealed class SafeProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SafeProcessHandle"/> class.
            /// </summary>
            /// <param name="preexistingHandle"> The preexisting handle. </param>
            /// <param name="ownsHandle"> The owns handle. </param>
            internal SafeProcessHandle(IntPtr preexistingHandle, bool ownsHandle)
                : base(ownsHandle)
            {
                this.SetHandle(preexistingHandle);
            }

            /// <summary>
            /// Prevents a default instance of the <see cref="SafeProcessHandle"/> class from being created.
            /// </summary>
            private SafeProcessHandle()
                : base(true)
            {
            }

            /// <summary>
            /// The release handle.
            /// </summary>
            /// <returns>
            /// The <see cref="bool"/>. </returns>
            protected override bool ReleaseHandle()
            {
                return NativeMethods.CloseHandle(this.handle);
            }
        }

        /// <summary>
        /// The file name from handle state.
        /// </summary>
        private class FileNameFromHandleState : IDisposable
        {
            /// <summary>
            /// The Logger.
            /// </summary>
            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            /// <summary>
            /// The mr.
            /// </summary>
            private ManualResetEvent mr;

            /// <summary>
            /// Initializes a new instance of the <see cref="FileNameFromHandleState"/> class.
            /// </summary>
            /// <param name="handle"> The handle. </param>
            public FileNameFromHandleState(IntPtr handle)
            {
                this.mr = new ManualResetEvent(false);
                this.Handle = handle;
            }

            /// <summary>
            /// Gets the handle.
            /// </summary>
            public IntPtr Handle { get; }

            /// <summary>
            /// Gets or sets the file name.
            /// </summary>
            public string FileName { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether ret value.
            /// </summary>
            public bool RetValue { get; set; }

            /// <summary>
            /// The wait one.
            /// </summary>
            /// <param name="wait"> The wait. </param>
            /// <returns> The <see cref="bool"/>. </returns>
            public bool WaitOne(int wait)
            {
                return this.mr.WaitOne(wait, false);
            }

            /// <summary>
            /// The set.
            /// </summary>
            public void Set()
            {
                this.mr?.Set();
            }

            #region IDisposable Members

            /// <summary>
            /// The dispose.
            /// </summary>
            public void Dispose()
            {
                this.mr?.Close();
            }

            #endregion
        }
    }
}
