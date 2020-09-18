
namespace UnityCommander.Test.WinApi
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// The native methods.
    /// </summary>
    public class NativeMethods
    {
        /// <summary>Retrieves the specified system information.</summary>
        /// <param name="SystemInformationClass"> Indicate the kind of system information to be retrieved. </param>
        /// <param name="SystemInformation">a buffer that receives the requested information</param>
        /// <param name="SystemInformationLength">The allocation size of the buffer pointed to by Info</param>
        /// <param name="ReturnLength">If null, ignored.  Otherwise tells you the size of the information returned by the kernel.</param>
        /// <returns>Status Information</returns>
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
        internal static extern MethodHelper.SafeProcessHandle OpenProcess(
            [In] ProcessAccessRights dwDesiredAccess,
            [In, MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
            [In] int dwProcessId);

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
}
