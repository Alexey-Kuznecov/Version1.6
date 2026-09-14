
using UnityCommander.Modules.FilePanel.Services;

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

            return new FilePanelNodeSet(
                factory.CreateFolderNode(),
                factory.CreateFileNode(),
                factory.CreateDriveNode(),
                factory.CreateHeaderNode());
        }
    }
}
