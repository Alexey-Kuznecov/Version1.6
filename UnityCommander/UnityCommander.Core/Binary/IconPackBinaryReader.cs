
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityCommander.Abstractions.Icons;

namespace UnityCommander.Core.Binary
{
    public static class IconPackBinaryReader
    {
        public static Dictionary<string, RuntimeIcon> Load(string path)
        {
            using var fs = File.OpenRead(path);
            using var br = new BinaryReader(fs, Encoding.UTF8);

            var version = br.ReadInt32();
            var count = br.ReadInt32();

            var result = new Dictionary<string, RuntimeIcon>(count, StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < count; i++)
            {
                var key = ReadString(br);
                var data = ReadString(br);

                string? color = null;

                var hasColor = br.ReadBoolean();
                if (hasColor)
                    color = ReadString(br);

                result[key] = new RuntimeIcon
                {
                    Key = key,
                    Data = data,
                    Color = color
                };
            }

            return result;
        }

        private static string ReadString(BinaryReader br)
        {
            var len = br.ReadInt32();
            var bytes = br.ReadBytes(len);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
