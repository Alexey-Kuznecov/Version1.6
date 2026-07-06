
namespace UnityCommander.Abstractions.Panels
{
    public interface IFileItem : IDirectoryItem
    {
        public long Size { get; set; }

        public string Extension { get; set; }
    }
}
