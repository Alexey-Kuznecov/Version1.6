
namespace UnityCommander.Search.Models
{
    public sealed class SearchItem
    {
        public string Path { get; }

        public SearchItem(string path)
        {
            Path = path;
        }
    }
}
