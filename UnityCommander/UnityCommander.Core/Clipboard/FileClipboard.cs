
using System.Collections.Generic;
using System.Linq;

namespace UnityCommander.Core.Clipboard
{
    public enum FileClipboardOperation
    {
        Copy,
        Cut
    }

    public sealed class FileClipboard
    {
        public IReadOnlyList<string> Paths { get; private set; } = [];

        public FileClipboardOperation? Operation { get; private set; }

        public bool HasItems => Paths.Count > 0;

        public void Set(
            IEnumerable<string> paths,
            FileClipboardOperation operation)
        {
            Paths = paths.ToArray();
            Operation = operation;
        }

        public void Clear()
        {
            Paths = [];
            Operation = null;
        }
    }
}
