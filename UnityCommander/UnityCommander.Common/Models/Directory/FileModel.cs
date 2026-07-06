
namespace UnityCommander.Common.Models.Directory
{
    using System;

    [Serializable]
    public class FileModel : BaseDirectory
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
