
namespace UnityCommander.Abstractions.IO
{
    public interface IFileActivityService
    {
        bool IsActive(Guid itemId);
        bool IsActive(string path);

        void Activate(
            Guid itemId,
            string sourcePath,
            string destinationPath);

        void Deactivate(Guid itemId);
    }
}
