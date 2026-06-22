using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityCommander.Rendering.Icons;

namespace UnityCommander.Core.Binary
{
    public static class IconPackBinaryWriter
    {
        public static void Save(string path, Dictionary<string, RuntimeIcon> icons)
        {
            using var fs = File.Create(path);
            using var bw = new BinaryWriter(fs, Encoding.UTF8, leaveOpen: false);

            bw.Write(1); // version
            bw.Write(icons.Count);

            foreach (var icon in icons.Values)
            {
                WriteString(bw, icon.Key);
                WriteString(bw, icon.Data);

                bw.Write(icon.Color is not null);

                if (icon.Color is not null)
                    WriteString(bw, icon.Color);
            }
        }

        private static void WriteString(BinaryWriter bw, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            bw.Write(bytes.Length);
            bw.Write(bytes);
        }
    }
}
