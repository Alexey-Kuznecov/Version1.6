
namespace UnityCommander.Abstractions.IO
{
    public interface IFileConflictResolver
    {
        Task<FileConflictAction> ResolveAsync(
            FileConflict conflict,
            CancellationToken cancellationToken = default);
    }
}
