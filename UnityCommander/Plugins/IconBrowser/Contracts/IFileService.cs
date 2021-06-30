namespace AIconBrowser.Contracts
{
    public interface IFileService
    {
        object Open(string path);
        void Save(string path, object obj);
    }
}
