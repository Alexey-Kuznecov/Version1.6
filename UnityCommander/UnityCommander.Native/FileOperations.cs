
namespace UnityCommander.Native
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using UnityCommander.Native.Api;

    /// <summary>
    /// The file operations.
    /// </summary>
    public class FileOperations
    {
        /// <summary>
        /// The pb cancel.
        /// </summary>
        private int pbCancel;

        /// <summary>
        /// The copy progress routine.
        /// </summary>
        /// <param name="totalFileSize"> The total size of the file, in bytes. </param>
        /// <param name="totalBytesTransferred"> The total number of bytes transferred from the source file to the destination file since the copy operation began. </param>
        /// <param name="streamSize"> The total size of the current file stream, in bytes. </param>
        /// <param name="streamBytesTransferred"> The total number of bytes in the current stream that have been transferred from the source file to the destination file since the copy operation began. </param>
        /// <param name="dwStreamNumber"> A handle to the current stream. The first time CopyProgressRoutine is called, the stream number is 1. </param>
        /// <param name="dwCallbackReason"> The reason that CopyProgressRoutine was called. This parameter can be one of the following values. <see cref="CopyProgressCallbackReason"/> </param>
        /// <param name="hSourceFile"> A handle to the source file. </param>
        /// <param name="hDestinationFile"> A handle to the destination file </param>
        /// <param name="lpData"> Argument passed to CopyProgressRoutine by CopyFileEx, MoveFileTransacted, or MoveFileWithProgress. </param>
        /// <returns> The CopyProgressRoutine function should return one of the following values. <see cref="CopyProgressResult"/> </returns>
        /// <remarks> Reference: https://docs.microsoft.com/en-us/windows/win32/api/winbase/nc-winbase-lpprogress_routine </remarks>
        public delegate CopyProgressResult CopyProgressRoutine(
            long totalFileSize,
            long totalBytesTransferred,
            long streamSize,
            long streamBytesTransferred,
            uint dwStreamNumber,
            CopyProgressCallbackReason dwCallbackReason,
            IntPtr hSourceFile,
            IntPtr hDestinationFile,
            IntPtr lpData);

        /// <summary>
        /// Copies an existing file to a new file. It is possible to pause, resume and cancel copying.
        /// </summary>
        /// <param name="oldFile"> The full path to a old file. </param>
        /// <param name="newFile"> The full path to a new file. </param>
        /// <param name="callback"> Delegate to control copy progress. </param>
        /// <returns> If the file is copied successfully, the return value is true. </returns>
        public bool XCopy(string oldFile, string newFile, CopyProgressRoutine callback)
        {
           return NativeFunctions.CopyFileEx(
                   oldFile, 
                   newFile,
                   callback,
                   IntPtr.Zero, 
                   ref this.pbCancel,
                   CopyFileFlags.COPY_FILE_RESTARTABLE);
        }

        /// <summary>
        /// Copies an existing file to a new file.
        /// </summary>
        /// <param name="oldFile"> The full path to a old file. </param>
        /// <param name="newFile"> The full path to a new file. </param>
        /// <returns> If the file is copied successfully, the return value is true. </returns>
        public bool XCopy(string oldFile, string newFile)
        {
            return NativeFunctions.CopyFileEx(
                oldFile,
                newFile,
                this.CopyProgressHandler,
                IntPtr.Zero,
                ref this.pbCancel,
                CopyFileFlags.COPY_FILE_RESTARTABLE);
        }

        /// <summary>
        /// The copy progress handler.
        /// </summary>
        /// <param name="total"> The total size of the file, in bytes. </param>
        /// <param name="transferred"> The total number of bytes transferred from the source file to the destination file since the copy operation began. </param>
        /// <param name="streamSize"> The total size of the current file stream, in bytes. </param>
        /// <param name="streamByteTrans"> The total number of bytes in the current stream that have been transferred from the source file to the destination file since the copy operation began. </param>
        /// <param name="dwStreamNumber"> A handle to the current stream. The first time CopyProgressRoutine is called, the stream number is 1. </param>
        /// <param name="reason"> The reason that CopyProgressRoutine was called. This parameter can be one of the following values. <see cref="CopyProgressCallbackReason"/> </param>
        /// <param name="hSourceFile"> A handle to the source file. </param>
        /// <param name="hDestinationFile"> A handle to the destination file </param>
        /// <param name="lpData"> Argument passed to CopyProgressRoutine by CopyFileEx, MoveFileTransacted, or MoveFileWithProgress. </param>
        /// <returns> The CopyProgressRoutine function should return one of the following values. <see cref="CopyProgressResult"/> </returns>
        public CopyProgressResult CopyProgressHandler(long total, long transferred, long streamSize, long streamByteTrans, uint dwStreamNumber, CopyProgressCallbackReason reason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData)
        {
            return CopyProgressResult.PROGRESS_CONTINUE;
        }
    }
}
