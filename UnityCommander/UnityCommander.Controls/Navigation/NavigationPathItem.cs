
namespace UnityCommander.Controls.Navigation
{
    public sealed class NavigationPathItem
    {
        public string Name { get; }
        public string Path { get; }
        public string ParentPath { get; }

        public NavigationPathItem(
            string name,
            string path,
            string parentPath)
        {
            Name = name;
            Path = path;
            ParentPath = parentPath;
        }
    }
}
