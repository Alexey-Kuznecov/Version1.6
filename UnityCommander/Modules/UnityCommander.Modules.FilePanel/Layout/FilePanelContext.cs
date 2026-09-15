
using UnityCommander.Core.Navigation;
using UnityCommander.Modules.FilePanel.Controllers;
using UnityCommander.Modules.FilePanel.States;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Selection;

namespace UnityCommander.Modules.FilePanel.Layout
{
    public sealed class FilePanelRuntime
    {
        public NavigationManager Navigation { get; }

        public FilePanelNodeBuilder NodeBuilder { get; }

        public Workspace Workspace { get; }

        public WorkspaceController WorkspaceController { get; }

        public FilePanelLayoutController LayoutController { get; }

        public FilePanelNodeSet Nodes { get; }

        public ISelectionManager SelectionManager { get; }

        public FilePanelRuntime(
            NavigationManager navigation,
            ISelectionManager selectionManager,
            FilePanelNodeBuilder nodeBuilder,
            Workspace workspace,
            WorkspaceController workspaceController,
            FilePanelLayoutController layoutController,
            FilePanelNodeSet nodes)
        {
            SelectionManager = selectionManager;
            Navigation = navigation;
            NodeBuilder = nodeBuilder;
            Workspace = workspace;
            WorkspaceController = workspaceController;
            LayoutController = layoutController;
            Nodes = nodes;
        }
    }
}
