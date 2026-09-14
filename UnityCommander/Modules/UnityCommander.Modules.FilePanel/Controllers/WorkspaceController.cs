
using System;
using UnityCommander.Controls.Layout;
using UnityCommander.Modules.FilePanel.Layout;
using UnityCommander.Modules.FilePanel.States;

namespace UnityCommander.Modules.FilePanel.Controllers
{
    public class WorkspaceController
    {
        private readonly Workspace _workspace;

        private FilePanelNodeSet? _nodes;

        public WorkspaceController(Workspace workspace)
        {
            _workspace = workspace;
        }

        public void SetNodes(FilePanelNodeSet nodes)
        {
            _nodes = nodes;
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

        public void Apply(FilePanelLayoutMode mode)
        {
            if (_nodes is null)
                throw new InvalidOperationException(
                    "File panel nodes have not been initialized.");

            switch (mode)
            {
                case FilePanelLayoutMode.Split:
                    ApplySplit();
                    break;

                case FilePanelLayoutMode.Unified:
                    ApplyUnified();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        private void ApplySplit()
        {
            _workspace.HeaderRegion.Content = _nodes!.Header;
            _workspace.MainRegion.Content = _nodes.Folder;
            _workspace.SecondaryRegion.Content = _nodes.File;
        }

        private void ApplyUnified()
        {
            _workspace.HeaderRegion.Content = _nodes!.Header;
            _workspace.MainRegion.Content = _nodes.Unified;
            _workspace.SecondaryRegion.Content = null;
        }
    }
}
