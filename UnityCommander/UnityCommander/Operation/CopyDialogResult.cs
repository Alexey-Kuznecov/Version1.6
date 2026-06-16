
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Common.Override.Engine;

namespace UnityCommander.Operation
{
    public sealed class CopyDialogResult : IDialogResult
    {
        public bool Accepted { get; init; }

        public FileOperationRequest Request { get; init; }
    }
}
