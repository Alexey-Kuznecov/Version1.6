
namespace UnityCommander.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using AlexeyKuznecov.Library.Converters;

    using UnityCommander.Common.Models.Directory;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The files provider.
    /// </summary>
    public class DataProviderService : IDataProviderService
    {
        public async Task<List<FileModel>> GetFilesAsync(string path)
        {
            return await Task.Run(() =>
            {
                var list = new List<FileModel>();
                var dir = new DirectoryInfo(path);

                foreach (var file in dir.GetFiles())
                {
                    if ((file.Attributes & FileAttributes.Hidden) == 0)
                    {
                        list.Add(new FileModel
                        {
                            Name = Path.GetFileNameWithoutExtension(file.Name),
                            Path = file.FullName,
                            Extension = file.Extension,
                            CreationTime = file.CreationTime,
                            LastAccessTime = file.LastAccessTime,
                            TargetPanel = TargetPanel.Files
                        });
                    }
                }

                return list;
            });
        }

        public async Task<List<FolderModel>> GetDirectoriesAsync(string path)
        {
            return await Task.Run(() =>
            {
                var list = new List<FolderModel>();
                var dir = new DirectoryInfo(path);

                foreach (var item in dir.GetDirectories())
                {
                    if ((item.Attributes & FileAttributes.Hidden) == 0)
                    {
                        list.Add(new FolderModel
                        {
                            Name = item.Name,
                            Path = item.FullName,
                            CreationTime = item.CreationTime,
                            LastAccessTime = item.LastAccessTime,
                            TargetPanel = TargetPanel.Folders
                        });
                    }
                }

                return list;
            });
        }

        public async Task<List<DriveModel>> GetDrivesAsync()
        {
            return await Task.Run(() =>
            {
                var list = new List<DriveModel>();

                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.DriveType == DriveType.Network) continue;
                    if (!drive.IsReady) continue;

                    list.Add(new DriveModel
                    {
                        Letter = drive.Name,
                        FreeSpace = ConverterBytes.AutoConvertFormatBytes(drive.AvailableFreeSpace),
                        UsedSpace = ConverterBytes.AutoConvertFormatBytes(drive.TotalSize - drive.AvailableFreeSpace),
                        TotalAmount = ConverterBytes.AutoConvertFormatBytes(drive.TotalSize),
                        TargetPanel = TargetPanel.LocalDisk
                    });
                }

                return list;
            });
        }
    }
}