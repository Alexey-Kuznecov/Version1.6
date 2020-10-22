
namespace UnityCommander.Native.Api
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Modified from original solution based on https://stackoverflow.com/a/9995536
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SYSTEM_HANDLE_ENTRY
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
    public struct UNICODE_STRING
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
    public struct OBJECT_TYPE_INFORMATION
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
    public struct OBJECT_NAME_INFORMATION
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
    /// The input/output counters.
    /// </summary>
    public struct IO_COUNTERS
    {
        /// <summary>
        /// The read operation count.
        /// </summary>
        public ulong ReadOperationCount;

        /// <summary>
        /// The write operation count.
        /// </summary>
        public ulong WriteOperationCount;

        /// <summary>
        /// The other operation count.
        /// </summary>
        public ulong OtherOperationCount;

        /// <summary>
        /// The read transfer count.
        /// </summary>
        public ulong ReadTransferCount;

        /// <summary>
        /// The write transfer count.
        /// </summary>
        public ulong WriteTransferCount;

        /// <summary>
        /// The other transfer count.
        /// </summary>
        public ulong OtherTransferCount;
    }
}
