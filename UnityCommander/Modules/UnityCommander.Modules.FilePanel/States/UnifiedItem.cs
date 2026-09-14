
using UnityCommander.Abstractions.Panels;

namespace UnityCommander.Modules.FilePanel.States
{
    public sealed class UnifiedItem
    {
        public UnifiedItemType Type { get; }

        public IDirectoryItem Item { get; }

        public UnifiedItem(
            UnifiedItemType type,
            IDirectoryItem item)
        {
            Type = type;
            Item = item;
        }

        public static UnifiedItem Folder(IDirectoryItem item) =>
            new(UnifiedItemType.Folder, item);

        public static UnifiedItem File(IDirectoryItem item) =>
            new(UnifiedItemType.File, item);
    }
}
