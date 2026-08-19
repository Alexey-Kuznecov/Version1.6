
namespace UnityCommander.Abstractions.IO
{
    public interface ICopyManager
    {
        Task CopyAsync(OperationContext ctx, string sourcePath, string targetPath);
    }
}
