
using UnityCommander.Core.Navigation;
using UnityCommander.Modules.FilePanel.States;

namespace UnityCommander.Modules.FilePanel.Layout
{
    public sealed class FilePanelRuntime
    {
        public NavigationManager Navigation { get; }

        public FilePanelNodeBuilder NodeBuilder { get; }

        public Workspace Workspace { get; }

        public FilePanelLayoutController LayoutController { get; }

        public FilePanelRuntime(
            NavigationManager navigation,
            FilePanelNodeBuilder nodeBuilder,
            Workspace workspace,
            FilePanelLayoutController layoutController)
        {
            Navigation = navigation;
            NodeBuilder = nodeBuilder;
            Workspace = workspace;
            LayoutController = layoutController;
        }
    }
}
