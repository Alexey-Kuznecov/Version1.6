
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Common.Panels;
using UnityCommander.Controls.Layout;
using UnityCommander.Core.Navigation;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Modules.FilePanel.Controllers;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Modules.FilePanel.States;
using UnityCommander.Services.Interfaces;
using UnityCommander.WPF.DragDrop;
using UnityCommander.WPF.Overlays;

namespace UnityCommander.Modules.FilePanel.Layout
{
    public sealed class FilePanelContextFactory
    {
        private readonly FilePanelLayoutService _layoutService;

        private readonly ActiveTabContext _activeTab;
        private readonly ICreationService _creationService;
        private readonly ContextMenuController _menu;
        private readonly ISelectionManager _selection;
        private readonly ICommandUIService _commands;
        private readonly GongDropAdapter _dropTarget;
        private readonly NodeContextRegistry _contextRegistry;
        private readonly ViewportMapper _scrollMapper;
        private readonly IWindowManager _windowManager;
        private readonly IPopupService _popupService;

        public FilePanelContextFactory(
            FilePanelLayoutService layoutService,
            ActiveTabContext activeTab,
            ICreationService creationService,
            ContextMenuController menu,
            ISelectionManager selection,
            ICommandUIService commands,
            GongDropAdapter dropTarget,
            NodeContextRegistry contextRegistry,
            ViewportMapper scrollMapper,
            IWindowManager windowManager,
            IPopupService popupService)
        {
            _layoutService = layoutService;
            _activeTab = activeTab;
            _creationService = creationService;
            _menu = menu;
            _selection = selection;
            _commands = commands;
            _dropTarget = dropTarget;
            _contextRegistry = contextRegistry;
            _scrollMapper = scrollMapper;
            _windowManager = windowManager;
            _popupService = popupService;
        }

        public FilePanelRuntime Create()
        {
            // 1. State of THIS panel.
            var workspace = new Workspace(
                new RegionNode(),
                new RegionNode(),
                new RegionNode());

            var workspaceController =
                new WorkspaceController(workspace);

            // 2. Navigation of THIS panel.
            var navigation = new NavigationManager(null);

            // 3. Layout controller of THIS panel.
            var layoutController =
                new FilePanelLayoutController(
                    _layoutService,
                    workspace,
                    workspaceController);

            // 4. Node infrastructure uses the same layout/navigation.
            var nodeContextFactory =
                new NodeContextFactory(
                    layoutController,
                    _creationService,
                    _menu,
                    navigation,
                    _selection,
                    _commands,
                    _dropTarget,
                    _contextRegistry,
                    _scrollMapper,
                    _windowManager,
                    _popupService,
                    _activeTab);

            var nodeBuilder =
                new FilePanelNodeBuilder(nodeContextFactory);

            // 5. Build all nodes once.
            var nodes = nodeBuilder.Build();

            // 6. Give WorkspaceController access to the available nodes.
            workspaceController.SetNodes(nodes);

            // 7. Apply initial layout/mode.
            layoutController.Initialize();

            return new FilePanelRuntime(
                navigation,
                _selection,
                nodeBuilder,
                workspace,
                workspaceController,
                layoutController,
                nodes);
        }
    }
}
