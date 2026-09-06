
using System;

namespace UnityCommander.Operation
{
    public class CopyOperationResult
    {
        public Guid OperationId { get; init; }
        public bool Success { get; init; }
        public int FilesCopied { get; init; }
        public string? Error { get; init; }
    }
}
