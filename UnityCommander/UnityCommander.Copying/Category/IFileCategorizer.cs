
using System.IO;

namespace UnityCommander.Copying.Category
{
    public interface IFileCategorizer
    {
        //string Categorize(FileInfo file); // возвращает имя подпапки или категории
        Task<string> CategorizeAsync(FileInfo file); // для тяжелых файлов
    }
}
