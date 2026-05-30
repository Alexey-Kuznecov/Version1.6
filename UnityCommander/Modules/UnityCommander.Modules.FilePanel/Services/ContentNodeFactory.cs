
using UnityCommander.Common.Models;
using UnityCommander.Modules.FilePanel.Layout;
using UnityCommander.Modules.FilePanel.Models;

namespace UnityCommander.Modules.FilePanel.Services
{
    public class ContentNodeFactory
    {
        private NodeContextFactory _contextFactory;

        public ContentNodeFactory(NodeContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ContentNode CreateFolderNode()
        {
            return new ContentNode
            {
                Role = ContentRole.Directory,
                ViewMode = ViewMode.Table,
                Context = _contextFactory.CreateFolderNode()
            };
        }

        public ContentNode CreateFileNode()
        {
            return new ContentNode
            {
                Role = ContentRole.File,
                ViewMode = ViewMode.Table,
                Context = _contextFactory.CreateFileNode()
            };
        }

        public ContentNode CreateDriveNode()
        {
            return new ContentNode
            {
                Role = ContentRole.Drive,
                ViewMode = ViewMode.Table,
                Context = _contextFactory.CreateDriveNode()
            };
        }

        public ContentNode CreateHeaderNode()
        {
            return new ContentNode
            {
                Role = ContentRole.Header,
                Context = _contextFactory.CreateHeaderNode()
            };
        }
    }
}
