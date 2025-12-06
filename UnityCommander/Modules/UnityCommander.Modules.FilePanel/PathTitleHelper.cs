using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Core.Navgator;

namespace UnityCommander.Modules.FilePanel
{
    public static class PathTitleHelper
    {
        /// <summary>
        /// Форматирует полный путь для отображения в заголовке вкладки.
        /// Ограничивает длину, оставляя последнюю папку видимой.
        /// </summary>
        /// <param name="fullPath">Полный путь к папке</param>
        /// <param name="maxLength">Максимальная длина заголовка</param>
        /// <returns>Строка для заголовка вкладки</returns>
        public static string GetTabTitle(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return "";

            if (fullPath == VirtualPaths.MyComputer)
                return fullPath;

            string drive = Path.GetPathRoot(fullPath)?.TrimEnd('\\') ?? "";
            string lastFolder = Path.GetFileName(fullPath.TrimEnd('\\')) ?? "";

            return $"[{drive}] {lastFolder}"; 
        }
    }
}
