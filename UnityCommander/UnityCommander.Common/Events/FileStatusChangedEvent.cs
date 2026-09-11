
using System;
using UnityCommander.Abstractions.IO;

namespace UnityCommander.Common.Events
{
    public sealed class FileStatusChangedEvent : EventArgs
    {
        public Guid ItemId { get; }
        public FileTransferStatus Status { get; }
        public string SourcePath { get; }
        public string DestinationPath { get; }

        public FileStatusChangedEvent(
            Guid itemId,
            FileTransferStatus status,
            string sourcePath,
            string destinationPath)
        {
            ItemId = itemId;
            Status = status;
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
        }
    }
}