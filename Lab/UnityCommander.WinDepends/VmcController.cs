
#define Nlog
#undef Nlog

namespace UnityCommander.WinDepends
{
    using System;
    using System.Collections.Generic;
    using System.EnterpriseServices;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;

    using Microsoft.Win32.SafeHandles;

    using NLog;
    using NLog.Config;

    #region ENUMs

    /// <summary>
    /// The nt status.
    /// </summary>
    internal enum NT_STATUS
    {
        /// <summary>
        /// The status success.
        /// </summary>
        STATUS_SUCCESS = 0x00000000,

        /// <summary>
        /// The status buffer overflow.
        /// </summary>
        STATUS_BUFFER_OVERFLOW = unchecked((int)0x80000005L),

        /// <summary>
        /// The info length is not sufficient to hold the data.
        /// </summary>
        STATUS_INFO_LENGTH_MISMATCH = unchecked((int)0xC0000004L)
    }

    /// <summary>
    /// The system information class.
    /// </summary>
    internal enum SYSTEM_INFORMATION_CLASS
    {
        /// <summary>
        /// The system handle information.
        /// </summary>
        SystemHandleInformation = 16,
    }

    /// <summary>
    /// The object information class.
    /// </summary>
    internal enum OBJECT_INFORMATION_CLASS
    {
        /// <summary>
        /// The object basic information.
        /// </summary>
        ObjectBasicInformation = 0,

        /// <summary>
        /// The object name information.
        /// </summary>
        ObjectNameInformation = 1,

        /// <summary>
        /// The object type information.
        /// </summary>
        ObjectTypeInformation = 2,

        /// <summary>
        /// The object all types information.
        /// </summary>
        ObjectAllTypesInformation = 3,

        /// <summary>
        /// The object handle information.
        /// </summary>
        ObjectHandleInformation = 4
    }

    /// <summary>
    /// The process access rights.
    /// </summary>
    [Flags]
    internal enum ProcessAccessRights
    {
        /// <summary>
        /// The process dup handle.
        /// </summary>
        PROCESS_DUP_HANDLE = 0x00000040
    }

    /// <summary>
    /// The duplicate handle options.
    /// </summary>
    [Flags]
    internal enum DuplicateHandleOptions
    {
        /// <summary>
        /// The duplicate close source.
        /// </summary>
        DUPLICATE_CLOSE_SOURCE = 0x1,

        /// <summary>
        /// The duplicate same access.
        /// </summary>
        DUPLICATE_SAME_ACCESS = 0x2
    }

    #endregion

    /// <summary>
    /// The detect open files.
    /// </summary>
    [ComVisible(true), EventTrackingEnabled(true)]
    public class DetectOpenFiles : ServicedComponent
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
        /// The Logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
        /// The system handle type.
        /// </summary>
        private enum SystemHandleType
        {
            /// <summary>
            /// The object type unknown.
            /// </summary>
            OB_TYPE_UNKNOWN = 0,

            /// <summary>
            /// The object type type.
            /// </summary>
            OB_TYPE_TYPE = 1,

            /// <summary>
            /// The object type directory.
            /// </summary>
            OB_TYPE_DIRECTORY,

            /// <summary>
            /// The object type symbolic link.
            /// </summary>
            OB_TYPE_SYMBOLIC_LINK,

            /// <summary>
            /// The object type token.
            /// </summary>
            OB_TYPE_TOKEN,

            /// <summary>
            /// The object type process.
            /// </summary>
            OB_TYPE_PROCESS,

            /// <summary>
            /// The object type thread.
            /// </summary>
            OB_TYPE_THREAD,

            /// <summary>
            /// The object type unknown 7.
            /// </summary>
            OB_TYPE_UNKNOWN_7,

            /// <summary>
            /// The object type event.
            /// </summary>
            OB_TYPE_EVENT,

            /// <summary>
            /// The object type event pair.
            /// </summary>
            OB_TYPE_EVENT_PAIR,

            /// <summary>
            /// The object type mutant.
            /// </summary>
            OB_TYPE_MUTANT,

            /// <summary>
            /// The object type unknown 11.
            /// </summary>
            OB_TYPE_UNKNOWN_11,

            /// <summary>
            /// The object type semaphore.
            /// </summary>
            OB_TYPE_SEMAPHORE,

            /// <summary>
            /// The object type timer.
            /// </summary>
            OB_TYPE_TIMER,

            /// <summary>
            /// The object type profile.
            /// </summary>
            OB_TYPE_PROFILE,

            /// <summary>
            /// The object type window station.
            /// </summary>
            OB_TYPE_WINDOW_STATION,

            /// <summary>
            /// The object type desktop.
            /// </summary>
            OB_TYPE_DESKTOP,

            /// <summary>
            /// The object type section.
            /// </summary>
            OB_TYPE_SECTION,

            /// <summary>
            /// The object type key.
            /// </summary>
            OB_TYPE_KEY,

            /// <summary>
            /// The object type port.
            /// </summary>
            OB_TYPE_PORT,

            /// <summary>
            /// The object type waitable port.
            /// </summary>
            OB_TYPE_WAITABLE_PORT,

            /// <summary>
            /// The object type unknown_21.
            /// </summary>
            OB_TYPE_UNKNOWN_21,

            /// <summary>
            /// The object type unknown_22.
            /// </summary>
            OB_TYPE_UNKNOWN_22,

            /// <summary>
            /// The object type unknown 23.
            /// </summary>
            OB_TYPE_UNKNOWN_23,

            /// <summary>
            /// The object type unknown 24.
            /// </summary>
            OB_TYPE_UNKNOWN_24,

            // OB_TYPE_CONTROLLER,
            // OB_TYPE_DEVICE,
            // OB_TYPE_DRIVER,

            /// <summary>
            /// The object type io completion.
            /// </summary>
            OB_TYPE_IO_COMPLETION,

            /// <summary>
            /// The o b_ typ e_ file.
            /// </summary>
            OB_TYPE_FILE
        }

        /// <summary>
        /// The get open files enumerator.
        /// </summary>
        /// <param name="processId"> The process id. </param>
        /// <returns> The <see cref="IEnumerator"/> </returns>
        public static IEnumerator<FileSystemInfo> GetOpenFilesEnumerator(int processId)
        {
#if (Nlog)
            Logger.Info("\n" + LogFormatter.DrawInfoHeader() + "\n");
#endif
            return new OpenFiles(processId).GetEnumerator();
        }

        #region Private Members

        /// <summary>
        /// The int ptr add.
        /// </summary>
        /// <param name="ptr"> The ptr. </param>
        /// <param name="offset"> The offset. </param>
        /// <returns> The <see cref="IntPtr"/>. </returns>
        private static IntPtr IntPtrAdd(IntPtr ptr, int offset)
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
        private static int GetProcessId(IntPtr processId)
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
        private static bool GetFileNameFromHandle(IntPtr handle, int processId, out string fileName)
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
                else
                {
                    fileName = string.Empty;
                    return false;
                }
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
                        Logger.Info("Memory allocation: {0} => {1} ", length, ptr.ToString());
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
        /// The get handle type.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <param name="processId"> The process id. </param>
        /// <param name="handleType"> The handle type. </param>
        /// <returns> The <see cref="bool"/>. </returns>
        private static bool GetHandleType(IntPtr handle, int processId, out SystemHandleType handleType)
        {
            string token = GetHandleTypeToken(handle, processId);
            return GetHandleTypeFromToken(token, out handleType);
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
        /// The convert device path to dos path.
        /// </summary>
        /// <param name="devicePath"> The device path. </param>
        /// <param name="dosPath"> The dos path. </param>
        /// <returns> The <see cref="bool"/>. </returns>
        private static bool ConvertDevicePathToDosPath(string devicePath, out string dosPath)
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

        #endregion

        /// <summary>
        /// Modified from original solution based on https://stackoverflow.com/a/9995536
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SYSTEM_HANDLE_ENTRY
        {
            /// <summary>
            /// The owner pid.
            /// </summary>
            public IntPtr OwnerPid;

            /// <summary>
            /// The object type.
            /// </summary>
            public byte ObjectType;

            /// <summary>
            /// The handle flags.
            /// </summary>
            public byte HandleFlags;

            /// <summary>
            /// The handle value.
            /// </summary>
            public short HandleValue;

            /// <summary>
            /// The object pointer.
            /// </summary>
            public IntPtr ObjectPointer;

            /// <summary>
            /// The access mask.
            /// </summary>
            public int AccessMask;
        }

        /// <summary>
        /// The unicode string.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct UNICODE_STRING
        {
            /// <summary>
            /// The length.
            /// </summary>
            public readonly ushort Length;

            /// <summary>
            /// The buffer.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public readonly string Buffer;

            /// <summary>
            /// The maximum length.
            /// </summary>
            private readonly ushort maximumLength;
        }

        /// <summary>
        /// The object type information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct OBJECT_TYPE_INFORMATION
        {
            /// <summary>
            /// The type name.
            /// </summary>
            public readonly UNICODE_STRING TypeName;

            /// <summary>
            /// The reserved.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
            public readonly ulong[] Reserved;
        }

        /// <summary>
        /// The object name information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct OBJECT_NAME_INFORMATION
        {
            /// <summary>
            /// The name.
            /// </summary>
            public readonly UNICODE_STRING Name;

            /// <summary>
            /// The name buffer.
            /// </summary>
            private readonly IntPtr nameBuffer;
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

        /// <summary>
        /// The open files.
        /// </summary>
        private sealed class OpenFiles : IEnumerable<FileSystemInfo>
        {
            /// <summary>
            /// The Logger.
            /// </summary>
            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            /// <summary>
            /// The process id.
            /// </summary>
            private readonly int processId;

            /// <summary>
            /// Initializes a new instance of the <see cref="OpenFiles"/> class.
            /// </summary>
            /// <param name="processId"> The process id. </param>
            internal OpenFiles(int processId)
            {
                LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Users\Alexey\Desktop\Version1.8\Lab\UnityCommander.WinDepends\NLog.config");
                this.processId = processId;
            }

            #region IEnumerable<FileSystemInfo> Members

            /// <summary>
            /// The get enumerator.
            /// </summary>
            /// <returns> The <see cref="IEnumerator"/>. </returns>
            public IEnumerator<FileSystemInfo> GetEnumerator()
            {
                NT_STATUS ret;
                int length = 0x10000;

                // Loop, probing for required memory.
                do
                {
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
                            // CER guarantees that the address of the allocated 
                            // memory is actually assigned to ptr if an 
                            // asynchronous exception occurs.
                            ptr = Marshal.AllocHGlobal(length);
#if (Nlog2)
                            Logger.Info("Memory allocation: \n> Pointer: {0} Usage: {1} EndPtr: {2} ", ptr, length, length + ptr.ToInt64());
#endif   
                        }
                        ret = NativeMethods.NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS.SystemHandleInformation, ptr, length, out var returnLength);

#if (Nlog2)
                        Logger.Info("\n> STATUS_SUCCESS: {0} \n> NT_STATUS.STATUS_INFO_LENGTH_MISMATCH: {0} ", ret == NT_STATUS.STATUS_SUCCESS, ret == NT_STATUS.STATUS_INFO_LENGTH_MISMATCH);
#endif

                        if (ret == NT_STATUS.STATUS_INFO_LENGTH_MISMATCH)
                        {
                            // Round required memory up to the nearest 64KB boundary.
                            length = (returnLength + 0xffff) & ~0xffff;
                        }
                        else if (ret == NT_STATUS.STATUS_SUCCESS)
                        {
                            int handleCount = Marshal.ReadInt32(ptr);
                            int offset = sizeof(int);
                            int size = Marshal.SizeOf(typeof(SYSTEM_HANDLE_ENTRY));

                            for (int i = 0; i < handleCount; i++)
                            {
                                SYSTEM_HANDLE_ENTRY handleEntry = (SYSTEM_HANDLE_ENTRY)Marshal.PtrToStructure(IntPtrAdd(ptr, offset), typeof(SYSTEM_HANDLE_ENTRY));

                                string hex = Marshal.PtrToStringAuto(handleEntry.ObjectPointer);
                                Logger.Info(hex);

                                int ownerProcessId = GetProcessId(handleEntry.OwnerPid);
                                if (ownerProcessId == this.processId)
                                {
                                    IntPtr handle = (IntPtr)handleEntry.HandleValue;
                                    SystemHandleType handleType;

                                    if (GetHandleType(handle, ownerProcessId, out handleType) && handleType == SystemHandleType.OB_TYPE_FILE)
                                    {
                                        if (GetFileNameFromHandle(handle, ownerProcessId, out var devicePath))
                                        {
                                            if (ConvertDevicePathToDosPath(devicePath, out var dosPath))
                                            {
#if (Nlog)
                                                Logger.Info("\n" + LogFormatter.DrawInfoHeader() + "\n");
                                                Logger.Info("\n" + LogFormatter.DrawObject(handleEntry) + "\n");
                                                Logger.Info("LOCAL VAR: \n> OwnerProcessId ({0}) \n> Handle ({1}) \n> HandleType [{2}] \n> DevicePath '{3}' \n> DosPath '{4}'  \n> Offset ({5}) \n> Size ({6})\n",
                                                    ownerProcessId,
                                                    handle, handleType,
                                                    devicePath,
                                                    dosPath,
                                                    offset,
                                                    size);
#endif

                                                if (File.Exists(dosPath))
                                                {
                                                    yield return new FileInfo(dosPath);
                                                }
                                                else if (Directory.Exists(dosPath))
                                                {
                                                    yield return new DirectoryInfo(dosPath);
                                                }
                                            }
                                        }
                                    }
                                }

                                offset += size;
                            }
                        }
                    }
                    finally
                    {
                        // CER guarantees that the allocated memory is freed, 
                        // if an asynchronous exception occurs. 
                        Marshal.FreeHGlobal(ptr);
                        //Logger.Info("Free Memory: {0} => {1} ", length, ptr.ToString());
                    }
                }
                while (ret == NT_STATUS.STATUS_INFO_LENGTH_MISMATCH);
            }

            #endregion

            #region IEnumerable Members

            /// <summary>
            /// The get enumerator.
            /// </summary>
            /// <returns> The <see cref="IEnumerator"/>. </returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
#if (Nlog)
                Logger.Info("\n" + LogFormatter.DrawInfoHeader() + "\n");
#endif
                return this.GetEnumerator();
            }

            #endregion
        }
    }

    #region Native Methods
    /// <summary>
    /// The native methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Retrieves the specified system information.
        /// See <see cref="https://docs.microsoft.com/en-us/windows/win32/api/winternl/nf-winternl-ntquerysysteminformation"/>
        /// </summary>
        /// <param name="SystemInformationClass">
        /// One of the values enumerated in SYSTEM_INFORMATION_CLASS, which indicate the kind of system information
        /// to be retrieved. These include the following values.
        /// </param>
        /// <param name="SystemInformation">
        /// A pointer to a buffer that receives the requested information.
        /// The size and structure of this information varies depending on the value of the SystemInformationClass parameter:
        /// </param>
        /// <param name="SystemInformationLength">
        /// The size of the buffer pointed to by the System Information Parameter, in bytes.
        /// </param>
        /// <param name="ReturnLength">
        /// An optional pointer to a location where the function writes the actual size of the information requested. If that size is less than or equal to the
        /// SystemInformationLength parameter, the function copies the information into the SystemInformation buffer; otherwise, it returns an NTSTATUS error code and returns in ReturnLength the size of buffer required to receive the requested information.
        /// </param>
        /// <returns>
        /// Returns an NTSTATUS success or error code. The forms and significance of NTSTATUS error codes are listed in the Ntstatus.h
        /// header file available in the DDK, and are described in the DDK documentation.
        /// </returns>
        [DllImport("ntdll.dll")]
        internal static extern NT_STATUS NtQuerySystemInformation(
            [In] SYSTEM_INFORMATION_CLASS SystemInformationClass,
            [In] IntPtr SystemInformation,
            [In] int SystemInformationLength,
            [Out] out int ReturnLength);

        /// <summary>
        /// Retrieves various kinds of object information.
        /// </summary>
        /// <param name="Handle"> The handle of the object for which information is being queried. </param>
        /// <param name="ObjectInformationClass">
        /// One of the following values, as enumerated in OBJECT_INFORMATION_CLASS,
        /// indicating the kind of object information to be retrieved.
        /// </param>
        /// <param name="ObjectInformation">
        /// An optional pointer to a buffer where the requested information is to be returned.
        /// The size and structure of this information varies depending on the value of the ObjectInformationClass parameter.
        /// </param>
        /// <param name="ObjectInformationLength">
        /// The size of the buffer pointed to by the ObjectInformation parameter, in bytes.
        /// </param>
        /// <param name="ReturnLength">
        /// An optional pointer to a location where the function writes the actual size of the information requested.
        /// If that size is less than or equal to the ObjectInformationLength parameter,
        /// the function copies the information into the ObjectInformation buffer; otherwise,
        /// it returns an NTSTATUS error code and returns in ReturnLength the size of the buffer
        /// required to receive the requested information.
        /// </param>
        /// <returns>
        /// Returns an NTSTATUS or error code.  The forms and significance of NTSTATUS error codes are listed in the Ntstatus.h
        /// header file available in the WDK, and are described in the WDK documentation.
        /// </returns>
        [DllImport("ntdll.dll")]
        internal static extern NT_STATUS NtQueryObject(
            [In] IntPtr Handle,
            [In] OBJECT_INFORMATION_CLASS ObjectInformationClass,
            [In] IntPtr ObjectInformation,
            [In] int ObjectInformationLength,
            [Out] out int ReturnLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern SafeProcessHandle OpenProcess(
            [In] ProcessAccessRights dwDesiredAccess,
            [In, MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
            [In] int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DuplicateHandle(
            [In] IntPtr hSourceProcessHandle,
            [In] IntPtr hSourceHandle,
            [In] IntPtr hTargetProcessHandle,
            [Out] out SafeObjectHandle lpTargetHandle,
            [In] int dwDesiredAccess,
            [In, MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
            [In] DuplicateHandleOptions dwOptions);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int GetProcessId([In] IntPtr Process);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]

        internal static extern bool CloseHandle([In] IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int QueryDosDevice(
            [In] string lpDeviceName,
            [Out] StringBuilder lpTargetPath,
            [In] int ucchMax);
    }

    #endregion

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
}