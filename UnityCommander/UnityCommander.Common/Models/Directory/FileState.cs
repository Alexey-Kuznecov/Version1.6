
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using UnityCommander.Abstractions.IO;

namespace UnityCommander.Common.Models
{
    public sealed class FileState : BindableBase, IFileState
    {
        public Guid OperationId;

        public string SourcePath { get; init; }

        public string DestinationPath { get; init; }

        public bool IsCopying { get; set; }

        private double _progress;

        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public long CurrentSize { get; set; }

        public long Speed { get; set; }

        public TimeSpan? RemainingTime { get; set; }

        public bool IsLocked { get; set; }

        public bool IsUploading { get; set; }

        public bool IsDownloading { get; set; }

        public bool HasConflicts { get; set; }

        public Dictionary<string, object> Values { get; } = new();

        public OperationStatus Status { get; internal set; }
    }
}
