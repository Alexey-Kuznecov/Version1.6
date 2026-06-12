using IconMaker.Core.ImportExport;
using IconMaker.Core.Models;
using System.IO;

namespace IconMaker.Core.Storage
{
    public sealed class JsonIconThemeStorage : IIconThemeStorage
    {
        private readonly IFileReader _reader;
        private readonly IFileWriter _writer;
        private readonly IThemeSerializer _serializer;

        private readonly string _directory;

        public JsonIconThemeStorage(
            string directory,
            IFileReader reader,
            IFileWriter writer,
            IThemeSerializer serializer)
        {
            _directory = directory;

            _reader = reader;
            _writer = writer;
            _serializer = serializer;
        }

        public IconTheme Load(string id)
        {
            var path = GetPath(id);

            var data = _reader.Read(path);

            return _serializer.Deserialize(data);
        }

        public void Save(IconTheme theme)
        {
            var path = GetPath(theme.Id);

            var data = _serializer.Serialize(theme);

            _writer.Write(path, data);
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
