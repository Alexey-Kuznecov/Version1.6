using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
