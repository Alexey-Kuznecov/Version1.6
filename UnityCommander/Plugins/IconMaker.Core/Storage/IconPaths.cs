
using System.IO;

namespace IconMaker.Core.Storage
{
    public sealed class IconPaths
    {
        public string RootPath { get; }

        public string DataPath => Path.Combine(RootPath, "Data");

        public IconPaths(string rootPath)
        {
            RootPath = rootPath;
        }
    }
}
