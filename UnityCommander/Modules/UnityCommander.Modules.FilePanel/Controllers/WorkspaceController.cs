
using UnityCommander.Controls.Layout;
using UnityCommander.Modules.FilePanel.States;

namespace UnityCommander.Modules.FilePanel.Controllers
{
    public class WorkspaceController
    {
        private readonly Workspace _workspace;

        public WorkspaceController(Workspace workspace)
        {
            _workspace = workspace;
        }

        public void ShowDirectoryMode(
            ContentNode headerNode,
            ContentNode folderNode,
            ContentNode fileNode)
        {
            _workspace.HeaderRegion.Content = headerNode;
            _workspace.MainRegion.Content = folderNode;
            _workspace.SecondaryRegion.Content = fileNode;
        }

        public void ShowUnifiedMode(
            ContentNode headerNode,
            ContentNode unifiedNode)
        {
            _workspace.HeaderRegion.Content = headerNode;
            _workspace.MainRegion.Content = unifiedNode;
            _workspace.SecondaryRegion.Content = null;
        }

        public void ShowMyComputerMode(
            ContentNode headerNode,
            ContentNode driveNode)
        {
            _workspace.HeaderRegion.Content = headerNode;
            _workspace.MainRegion.Content = driveNode;
            _workspace.SecondaryRegion.Content = null;
        }

        public void ShowPreviewMode(
            ContentNode driveNode,
            ContentNode previewNode)
        {
            _workspace.MainRegion.Content = driveNode;

            _workspace.SecondaryRegion.Content = previewNode;
        }
    }
}
