using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Modules.FilePanel.Selection
{
    public class FileSystemItem
    {
        public string FullPath { get; set; }
        public string Name => Path.GetFileName(FullPath);
        public bool IsDirectory { get; set; }
        // другие метаданные: size, icon, etc.
        public override bool Equals(object obj) => obj is FileSystemItem f && string.Equals(f.FullPath, FullPath, StringComparison.OrdinalIgnoreCase);
        public override int GetHashCode() => FullPath?.ToLowerInvariant().GetHashCode() ?? 0;
    }
}
