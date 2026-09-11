
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Abstractions.IO;

namespace UnityCommander.Operation
{
    public sealed class FileConflictResult : IDialogResult
    {
        public FileConflictAction Action { get; init; }

        public bool ApplyToAll { get; init; }
    }
}
