
using System.Collections.Generic;
using UnityCommander.Abstractions.Icons;

namespace UnityCommander.Core.Binary
{
    public static class IconPackBuilder
    {
        public static void Save(string path, Dictionary<string, RuntimeIcon> icons)
        {
            IconPackBinaryWriter.Save(path, icons);
        }
    }
}
