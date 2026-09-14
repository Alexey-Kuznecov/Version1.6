
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Modules.FilePanel.States;

namespace UnityCommander.Modules.FilePanel.Layout
{
    public sealed class FilePanelNodeBuilder
    {
        private readonly NodeContextFactory _contextFactory;

        public FilePanelNodeBuilder(NodeContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public FilePanelNodeSet Build()
        {
            var factory = new ContentNodeFactory(_contextFactory);

            var folder = factory.CreateFolderNode();
            var file = factory.CreateFileNode();
            var drive = factory.CreateDriveNode();
            var header = factory.CreateHeaderNode();

            var folderContext = (FolderNodeContext)folder.Context;
            var fileContext = (FileNodeContext)file.Context;

            var unified = factory.CreateUnifiedNode(
                folderContext,
                fileContext);

            return new FilePanelNodeSet(
                folder,
                file,
                drive,
                header,
                unified);
        }
    }
}
