
namespace UnityCommander.Abstractions
{
    public interface IFileLauncher
    {
        void Open(string path);

        void OpenWith(string path);

        void OpenLocation(string path);
    }
}
