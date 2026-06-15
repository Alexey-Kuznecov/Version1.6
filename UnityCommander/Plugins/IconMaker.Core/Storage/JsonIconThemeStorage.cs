using IconMaker.Core.ImportExport;
using IconMaker.Core.Models;
using System.IO;

namespace IconMaker.Core.Storage
{
    public sealed class JsonIconThemeStorage : IIconThemeStorage
    {
        private readonly FileSystem _file;

        private readonly IThemeSerializer _serializer;

        private readonly string _directory;

        public JsonIconThemeStorage(
            IconPaths iconPaths,
            FileSystem file,
            IThemeSerializer serializer)
        {
            _directory = iconPaths.RootPath;
            _file = file;
            _serializer = serializer;
        }

        public IconTheme Load(string id)
        {
            var path = GetPath(id);

            var data = _file.Read(path);

            return _serializer.Deserialize(data);
        }

        public void Save(IconTheme theme)
        {
            var path = GetPath(theme.Id);

            var data = _serializer.Serialize(theme);

            _file.Write(path, data);
        }

        public void Delete(string id)
        {
            var path = GetPath(id);

            if (File.Exists(path))
                File.Delete(path);
        }

        private string GetPath(string id)
        {
            return Path.Combine(_directory, $"{id}.theme.json");
        }
    }
}
