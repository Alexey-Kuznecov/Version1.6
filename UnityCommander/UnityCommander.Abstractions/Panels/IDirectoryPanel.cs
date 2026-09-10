
namespace UnityCommander.Abstractions.Panels
{
    using System.Collections.Generic;

    public interface IDirectoryPanel : ITabPanelContent
    {
        IReadOnlyList<IFileItem> GetFiles();

        IReadOnlyList<IFolderItem> GetDirectories();

        public IFileNodeContext FileContext { get; }

        public IFolderNodeContext FolderContext { get; }
    }
}
