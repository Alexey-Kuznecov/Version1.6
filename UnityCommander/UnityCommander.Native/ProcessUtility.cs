
namespace UnityCommander.Native
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    using UnityCommander.Native.Api;

    /// <summary>
    /// Class serves to search all files handle is opened of the selected process. 
    /// </summary>
    public sealed class ProcessUtility : IEnumerable<FileSystemInfo>
    {
        /// <summary>
        /// The process id.
        /// </summary>
        private readonly int processId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessUtility"/> class.
        /// </summary>
        /// <param name="processId"> The process id. </param>
        public ProcessUtility(int processId)
        {
            this.processId = processId;
        }

        /// <summary>
        /// The get open files enumerator.
        /// </summary>
        /// <param name="processId"> The process id. </param>
        /// <returns> The <see cref="IEnumerator"/> </returns>
        public static IEnumerator<FileSystemInfo> GetOpenFilesEnumerator(int processId)
        {
            return new ProcessUtility(processId).GetEnumerator();
        }

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
                    }

                    ret = NativeFunctions.NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS.SystemHandleInformation, ptr, length, out var returnLength);

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
                            SYSTEM_HANDLE_ENTRY handleEntry = (SYSTEM_HANDLE_ENTRY)Marshal.PtrToStructure(MethodHelper.IntPtrAdd(ptr, offset), typeof(SYSTEM_HANDLE_ENTRY));

                            int ownerProcessId = MethodHelper.GetProcessId(handleEntry.OwnerPid);
                            if (ownerProcessId == this.processId)
                            {
                                IntPtr handle = (IntPtr)handleEntry.HandleValue;
                                SystemHandleType handleType;

                                if (MethodHelper.GetHandleType(handle, ownerProcessId, out handleType) && handleType == SystemHandleType.OB_TYPE_FILE)
                                {
                                    if (MethodHelper.GetFileNameFromHandle(handle, ownerProcessId, out var devicePath))
                                    {
                                        if (MethodHelper.ConvertDevicePathToDosPath(devicePath, out var dosPath))
                                        {
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
                }
            }
            while (ret == NT_STATUS.STATUS_INFO_LENGTH_MISMATCH);
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns> The <see cref="IEnumerator"/>. </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}