
namespace UnityCommander.Common
{
    // ModuleC - Интерфейс для операций над файлами
    public interface IFileOperations
    {
        void Move(string source, string destination);
        void Create(string filePath);
        void Delete(string filePath);
    }
}
