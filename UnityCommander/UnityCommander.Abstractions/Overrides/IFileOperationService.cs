namespace UnityCommander.Abstractions.Overrides
{
    public interface IFileOperationService
    {
        Task CopyAsync(FileOperationRequest request);
    }
}
