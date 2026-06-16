
namespace UnityCommander.Operation
{
    public class CopyOperationResult
    {
        public bool Success { get; init; }
        public int FilesCopied { get; init; }
        public string? Error { get; init; }
    }
}
