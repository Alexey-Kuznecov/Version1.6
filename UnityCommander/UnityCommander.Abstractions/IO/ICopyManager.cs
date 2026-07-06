
namespace UnityCommander.Abstractions.IO
{
    public interface ICopyManager
    {
        Task CopyAsync(Guid id, string sourcePath, string targetPath);
    }
}
