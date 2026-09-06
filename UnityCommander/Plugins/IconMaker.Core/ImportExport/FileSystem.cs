using System.IO;

namespace IconMaker.Core.ImportExport
{
    public class FileSystem : IFileReader, IFileWriter
    {
        public string Read(string path)
            => File.ReadAllText(path);

        public void Write(string path, string content)
            => File.WriteAllText(path, content);
    }
}
