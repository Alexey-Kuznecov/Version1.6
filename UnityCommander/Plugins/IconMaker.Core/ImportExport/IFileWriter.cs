namespace IconMaker.Core.ImportExport
{
    public interface IFileWriter
    {
        void Write(string path, string content);
    }
}
