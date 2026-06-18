
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Abstractions.Overrides;

namespace UnityCommander.Operation
{
    public sealed class CopyDialogResult : IDialogResult
    {
        public bool Accepted { get; init; }

        public FileOperationRequest Request { get; init; }
    }
}
