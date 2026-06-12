
using IconMaker.Core.ImportExport;
using IconMaker.Core.Models;
using System.IO;

namespace IconMaker.Core.Storage
{
    public class JsonIconStorage : IIconStorage
    {
        private readonly IIconSerializer _serializer;
        private readonly FileSystem _file;
        private string _rootPath;

        public JsonIconStorage(
             string packSource,
             FileSystem file,
             IIconSerializer serializer)
        {
            _rootPath = packSource;
            _serializer = serializer;
            _file = file;
        }

        public void Delete(string name)
        {
            var path = GetPath(name);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public IEnumerable<string> GetPackIds()
        {
            foreach (var file in Directory.GetFiles(_rootPath, "*.json"))
            {
                yield return Path.GetFileNameWithoutExtension(file);
            }
        }

        public IconPack Load(string name)
        {
            var path = GetPath(name);

            var data = _file.Read(path);

            return _serializer.Deserialize(data);
        }

        public void Save(IconPack pack)
        {
            var path = GetPath(pack.Id);

            var data = _serializer.Serialize(pack);

            _file.Write(path, data);
        }

        private string GetPath(string name)
            => Path.Combine(_rootPath, $"{name}.json");
    }
}
