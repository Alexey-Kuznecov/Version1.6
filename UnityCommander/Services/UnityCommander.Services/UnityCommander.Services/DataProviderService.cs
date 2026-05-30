
namespace UnityCommander.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using UnityCommander.Common.Models.Directory;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// Сервис получения данных с диска — формирует готовые модели для приложения.
    /// </summary>
    public class DataProviderService : IDataProviderService
    {
        /// <summary>
        /// Получить файлы в директории.
        /// </summary>
        public Task<List<FileModel>> GetFilesAsync(string path)
        {
            var dir = new DirectoryInfo(path);
            var files = new List<FileModel>();

            foreach (var file in dir.GetFiles())
            {
                if ((file.Attributes & FileAttributes.Hidden) == 0)
                {
                    files.Add(new FileModel
                    {
                        Name = Path.GetFileNameWithoutExtension(file.Name),
                        Path = file.FullName,
                        Extension = file.Extension,
                        CreationTime = file.CreationTime,
                        LastAccessTime = file.LastAccessTime,
                        TargetPanel = TargetPanel.Files,
                        Key = file.FullName,
                        Size = file.Length,
                    });
                }
            }

            return Task.FromResult(files);
        }

        /// <summary>
        /// Получить папки в директории.
        /// </summary>
        public Task<List<FolderModel>> GetDirectoriesAsync(string path)
        {
            var dir = new DirectoryInfo(path);
            var folders = new List<FolderModel>();

            foreach (var folder in dir.EnumerateDirectories())
            {
                try
                {
                    if ((folder.Attributes & FileAttributes.Hidden) != 0)
                        continue;

                    folders.Add(new FolderModel
                    {
                        Name = folder.Name,
                        Path = folder.FullName,
                        CreationTime = folder.CreationTime,
                        LastAccessTime = folder.LastAccessTime,
                        TargetPanel = TargetPanel.Folders,
                        Key = folder.FullName
                    });
                }
                catch
                {
                    // иногда доступ к папке может падать (system/permission)
                }
            }

            return Task.FromResult(folders);
        }

        /// <summary>
        /// Получить локальные диски.
        /// </summary>
        public async Task<List<DriveModel>> GetDrivesAsync()
        {
            return await Task.Run(() =>
            {
                var drives = new List<DriveModel>();

                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (!drive.IsReady) continue;
                    if (drive.DriveType == DriveType.Network) continue;

                    drives.Add(new DriveModel
                    {
                        Letter = drive.Name,
                        FreeSpace = drive.AvailableFreeSpace,   // сырые байты
                        UsedSpace = drive.TotalSize - drive.AvailableFreeSpace, // сырые байты
                        TotalAmount = drive.TotalSize,          // сырые байты
                        TargetPanel = TargetPanel.LocalDisk
                    });
                }

                return drives;
            });
        }
    }
}