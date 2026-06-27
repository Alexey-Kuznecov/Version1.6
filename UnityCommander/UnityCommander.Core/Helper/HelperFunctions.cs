
using System;

namespace UnityCommander.Core.Helper
{
    public static class HelperFunctions
    {
        public static string[] ParsePath(string dirPath)
        {
            string[] splitPath = dirPath.Split('\\');
            string[] paths = new string[!splitPath.Contains(string.Empty) ? splitPath.Length : splitPath.Length - 1];
            string newPath = string.Empty;
            int i = 0;

            while (i < splitPath.Length)
            {
                if (splitPath[i] == string.Empty) break;

                newPath = newPath.Equals(string.Empty)
                              ? splitPath[i] + "\\"
                              : newPath.Replace(paths[i - 1], newPath + splitPath[i] + "\\");

                paths[i++] = newPath;
            }

            return paths;
        }
    }
}
