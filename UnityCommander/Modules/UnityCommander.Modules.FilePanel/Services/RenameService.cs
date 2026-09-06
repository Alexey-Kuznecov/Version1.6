
using System.IO;
using System.Threading.Tasks;

namespace UnityCommander.Modules.FilePanel.Services
{
    public sealed class RenameService : IRenameService
    {
        public Task RenameAsync(
            string sourcePath,
            string newName)
        {
            var directory = Path.GetDirectoryName(sourcePath)
                ?? throw new IOException(
                    $"Cannot determine parent directory: {sourcePath}");

            var targetPath = Path.Combine(directory, newName);

            if (File.Exists(targetPath) ||
                Directory.Exists(targetPath))
            {
                throw new IOException(
                    $"Object already exists: {targetPath}");
            }

            if (File.Exists(sourcePath))
            {
                File.Move(sourcePath, targetPath);
            }
            else if (Directory.Exists(sourcePath))
            {
                Directory.Move(sourcePath, targetPath);
            }
            else
            {
                throw new FileNotFoundException(
                    "Object not found.",
                    sourcePath);
            }

            return Task.CompletedTask;
        }
    }
}
