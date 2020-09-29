
namespace UnityCommander.Native.Api
{
    using System;

    #region Files Operation

    /// <summary>
    /// The copy progress result.
    /// </summary>
    public enum CopyProgressResult : uint
    {
        /// <summary>
        /// Continue the copy operation.
        /// </summary>
        PROGRESS_CONTINUE = 0,

        /// <summary>
        /// Cancel the copy operation and delete the destination file.
        /// </summary>
        PROGRESS_CANCEL = 1,

        /// <summary>
        /// Stop the copy operation. It can be restarted at a later time.
        /// </summary>
        PROGRESS_STOP = 2,

        /// <summary>
        /// Continue the copy operation, but stop invoking CopyProgressRoutine to report progress.
        /// </summary>
        PROGRESS_QUIET = 3
    }

    /// <summary>
    /// The reason that CopyProgressRoutine was called. This parameter can be one of the following values.
    /// </summary>
    public enum CopyProgressCallbackReason : uint
    {
        /// <summary>
        /// Another part of the data file was copied.
        /// </summary>
        CALLBACK_CHUNK_FINISHED = 0x00000000,

        /// <summary>
        /// Another stream was created and is about to be copied.
        /// This is the callback reason given when the callback routine is first invoked.
        /// </summary>
        CALLBACK_STREAM_SWITCH = 0x00000001
    }

    /// <summary>
    /// The copy file flags.
    /// </summary>
    [Flags]
    public enum CopyFileFlags : uint
    {
        /// <summary>
        /// The copy operation fails immediately if the target file already exists.
        /// </summary>
        COPY_FILE_FAIL_IF_EXISTS = 0x00000001,

        /// <summary>
        /// The file is copied and the original file is opened for write access.
        /// </summary>
        COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004,

        /// <summary>
        /// An attempt to copy an encrypted file will succeed even
        /// if the destination copy cannot be encrypted.
        /// </summary>
        COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008,

        /// <summary>
        /// The copy operation is performed using unbuffered I/O, bypassing system I/O cache resources.
        /// Recommended for very large file transfers.
        /// </summary>
        COPY_FILE_NO_BUFFERING = 0x00001000,

        /// <summary>
        /// If the source file is a symbolic link, the destination file is also
        /// a symbolic link pointing to the same file that the source symbolic
        /// link is pointing to.
        /// </summary>
        COPY_FILE_COPY_SYMLINK = 0x00000800,

        /// <summary>
        /// Progress of the copy is tracked in the target file in case the copy fails.
        /// The failed copy can be restarted at a later time by specifying the same values
        /// for lpExistingFileName and lpNewFileName as those used in the call that failed.
        /// This can significantly slow down the copy operation as the new file may be flushed
        /// multiple times during the copy operation.
        /// </summary>
        COPY_FILE_RESTARTABLE = 0x00000002
    }

    #endregion

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
    /// By combining the NTSTATUS into a single 32-bit numbering space,
    /// the following NTSTATUS values are defined. Most values also have a defined default message
    /// that can be used to map the value to a human-readable text message.
    /// When this is done, the NTSTATUS value is also known as a message identifier.
    /// </summary>
    /// <remarks> Reference: https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-erref/596a1078-e883-4972-9bbc-49e60bebca55 </remarks> 
    internal enum NT_STATUS
    {
        /// <summary>
        /// The operation completed successfully.
        /// </summary>
        STATUS_SUCCESS = 0x00000000,

        /// <summary>
        /// The data was too large to fit into the specified buffer.
        /// </summary>
        STATUS_BUFFER_OVERFLOW = unchecked((int)0x80000005L),

        /// <summary>
        /// The specified information record length does not match the length that is required for the specified information class.
        /// </summary>
        STATUS_INFO_LENGTH_MISMATCH = unchecked((int)0xC0000004L)
    }

    /// <summary>
    /// One of the following values, as enumerated in OBJECT_INFORMATION_CLASS,
    /// indicating the kind of object information to be retrieved.
    /// </summary>
    internal enum OBJECT_INFORMATION_CLASS
    {
        /// <summary>
        /// The object basic information.
        /// </summary>
        ObjectBasicInformation = 0,

        /// <summary>
        /// Returns a <see cref="OBJECT_NAME_INFORMATION"/> structure.
        /// </summary>
        ObjectNameInformation = 1,

        /// <summary>
        /// Returns a <see cref="OBJECT_TYPE_INFORMATION"/> structure.
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
