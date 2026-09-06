
namespace UnityCommander.Common.Models.Directory
{
    using System;
    using UnityCommander.Abstractions.Panels;

    [Serializable]
    public class FileModel : BaseDirectory, IFileItem
    {
        private long _size;

        public long Size
        {
            get => _size;
            set => SetProperty(ref _size, value);
        }

        private string _extension;

        public string Extension
        {
            get => _extension;
            set => SetProperty(ref _extension, value);
        }
    }
}
