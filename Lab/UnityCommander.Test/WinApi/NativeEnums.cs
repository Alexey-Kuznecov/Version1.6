
namespace UnityCommander.Test.WinApi
{
    using System;

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

    /// <summary>
    /// The system handle type.
    /// </summary>
    internal enum SystemHandleType
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
}
