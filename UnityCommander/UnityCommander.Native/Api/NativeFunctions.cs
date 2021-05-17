
namespace UnityCommander.Native.Api
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// The native function of the win api.
    /// </summary>
    public class NativeFunctions
    {
        #region Developer Notes

        /// <summary> Retrieves the specified system information. </summary>
        /// <param name="systemInformationClass"> Indicate the kind of system information to be retrieved. </param>
        /// <param name="systemInformation"> A buffer that receives the requested information. </param>
        /// <param name="systemInformationLength"> The allocation size of the buffer pointed to by Info. </param>
        /// <param name="returnLength"> If null, ignored. Otherwise tells you the size of the information returned by the kernel. </param>
        /// <returns> Returns an <see cref="NT_STATUS"/> or error code. </returns>
        /// <remarks> Reference: https://docs.microsoft.com/en-us/windows/win32/api/winternl/nf-winternl-ntquerysysteminformation. </remarks>
        [DllImport("ntdll.dll")]
        internal static extern NT_STATUS NtQuerySystemInformation(
            [In] SYSTEM_INFORMATION_CLASS systemInformationClass,
            [In] IntPtr systemInformation,
            [In] int systemInformationLength,
            [Out] out int returnLength);

        /// <summary>
        /// Retrieves various kinds of object information.
        /// </summary>
        /// <param name="handle"> The handle of the object for which information is being queried. </param>
        /// <param name="objectInformationClass"> One of the following values, as enumerated in OBJECT_INFORMATION_CLASS, indicating the kind of object information to be retrieved. </param>
        /// <param name="objectInformation"> An optional pointer to a buffer where the requested information is to be returned. </param>
        /// <param name="objectInformationLength"> The size of the buffer pointed to by the objectInformation parameter, in bytes. </param>
        /// <param name="returnLength"> An optional pointer to a location where the function writes the actual size of the information requested. </param>
        /// <returns> Returns an <see cref="NT_STATUS"/> or error code. </returns>
        /// <remarks> Reference: https://docs.microsoft.com/en-us/windows/win32/api/winternl/nf-winternl-ntqueryobject. </remarks>
        [DllImport("ntdll.dll")]
        internal static extern NT_STATUS NtQueryObject(
            [In] IntPtr handle,
            [In] OBJECT_INFORMATION_CLASS objectInformationClass,
            [In] IntPtr objectInformation,
            [In] int objectInformationLength,
            [Out] out int returnLength);

        #endregion

        #region Process Functions

        /// <summary>
        /// Opens an existing local process object.
        /// </summary>
        /// <param name="dwDesiredAccess"> The access to the process object. This access right is checked against the security descriptor for the process. This parameter can be one or more of the process access rights. </param>
        /// <param name="bInheritHandle"> If the caller has enabled the SeDebugPrivilege privilege, the requested access is granted regardless of the contents of the security descriptor. </param>
        /// <param name="dwProcessId"> If this value is TRUE, processes created by this process will inherit the handle. Otherwise, the processes do not inherit this handle.</param>
        /// <returns> If the function succeeds, the return value is an open handle to the specified process. </returns>
        /// <remarks> Reference: https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-openprocess </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern MethodHelper.SafeProcessHandle OpenProcess(
            [In] ProcessAccessRights dwDesiredAccess,
            [In, MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
            [In] int dwProcessId);

        /// <summary>
        /// Retrieves a pseudo handle for the current process.
        /// </summary>
        /// <returns> The return value is a pseudo handle to the current process. </returns>
        /// <remarks> Reference: https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-getcurrentprocess </remarks>
        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetCurrentProcess();

        /// <summary>
        /// Retrieves the process identifier of the specified process.
        /// </summary>
        /// <param name="process"> A handle to the process. </param>
        /// <returns> If the function succeeds, the return value is the process identifier. </returns>
        /// <remarks> Reference: https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-getprocessid </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int GetProcessId(
            [In] IntPtr process);

        /// <summary>
        /// The get foreground window.
        /// </summary>
        /// <returns> The <see cref="IntPtr"/>. </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// The get window thread process id.
        /// </summary>
        /// <param name="hwnd"> The hwnd. </param>
        /// <param name="pid"> The pid. </param>
        /// <returns> The <see cref="uint"/>. </returns>
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hwnd, ref int pid);

        #endregion

        #region System Services

        /// <summary>
        /// Duplicates an object handle.
        /// </summary>
        /// <param name="hSourceProcessHandle"> A handle to the process with the handle to be duplicated. </param>
        /// <param name="hSourceHandle"> The handle to be duplicated. This is an open object handle that is valid in the context of the source process. </param>
        /// <param name="hTargetProcessHandle"> A handle to the process that is to receive the duplicated handle. </param>
        /// <param name="lpTargetHandle"> A pointer to a variable that receives the duplicate handle. </param>
        /// <param name="dwDesiredAccess"> The access requested for the new handle. </param>
        /// <param name="bInheritHandle"> This parameter is ignored if the dwOptions parameter specifies the DUPLICATE_SAME_ACCESS flag. </param>
        /// <param name="dwOptions"> A variable that indicates whether the handle is inheritable. </param>
        /// <returns> If the function succeeds, the return value is nonzero. </returns>
        /// <remarks> Reference: https://docs.microsoft.com/en-us/windows/win32/api/handleapi/nf-handleapi-duplicatehandle </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DuplicateHandle(
            [In] IntPtr hSourceProcessHandle,
            [In] IntPtr hSourceHandle,
            [In] IntPtr hTargetProcessHandle,
            [Out] out MethodHelper.SafeObjectHandle lpTargetHandle,
            [In] int dwDesiredAccess,
            [In, MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
            [In] DuplicateHandleOptions dwOptions);

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="hObject"> A valid handle to an open object. </param>
        /// <returns> If the function succeeds, the return value is nonzero. </returns>
        /// <remarks> Reference: https://docs.microsoft.com/en-us/windows/win32/api/handleapi/nf-handleapi-closehandle </remarks>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle([In] IntPtr hObject);

        #endregion

        #region Data Access and Storage 

        /// <summary>
        /// Retrieves information about MS-DOS device names. The function can obtain the current mapping for a particular MS-DOS device name.
        /// The function can also obtain a list of all existing MS-DOS device names.
        /// </summary>
        /// <param name="lpDeviceName">
        /// An MS-DOS device name string specifying the target of the query.
        /// The device name cannot have a trailing backslash; for example, use "C:", not "C:\".
        /// </param>
        /// <param name="lpTargetPath"> A pointer to a buffer that will receive the result of the query. </param>
        /// <param name="ucchMax"> The maximum number of TCHARs that can be stored into the buffer pointed to by lpTargetPath. </param>
        /// <returns> If the function succeeds, the return value is the number of TCHARs stored into the buffer pointed to by lpTargetPath. </returns>
        /// <remarks> Reference: https://docs.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-querydosdevicew </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int QueryDosDevice(
            [In] string lpDeviceName,
            [Out] StringBuilder lpTargetPath,
            [In] int ucchMax);

        #endregion

        #region Files Functions

        /// <summary>
        /// Copies an existing file to a new file, notifying the application of its progress through a callback function.
        /// </summary>
        /// <param name="lpExistingFileName"> The name of an existing file. </param>
        /// <param name="lpNewFileName"> The name of the new file. </param>
        /// <param name="lpProgressRoutine"> The address of a callback function of type LP_PROGRESS_ROUTINE that is called each time another portion of the file has been copied. </param>
        /// <param name="lpData"> The argument to be passed to the callback function. This parameter can be NULL. </param>
        /// <param name="pbCancel"> If this flag is set to TRUE during the copy operation, the operation is canceled. Otherwise, the copy operation will continue to completion. </param>
        /// <param name="dwCopyFlags"> Flags that specify how the file is to be copied. </param>
        /// <returns> If the function succeeds, the return value is nonzero. </returns>
        /// <remarks> Reference: https://docs.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-copyfileexw </remarks>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CopyFileEx(
            string lpExistingFileName,
            string lpNewFileName, 
            FileOperations.CopyProgressRoutine lpProgressRoutine,
            IntPtr lpData, 
            ref int pbCancel,
            CopyFileFlags dwCopyFlags);

        #endregion
    }
}
