
using System;
using UnityCommander.Controls.Layout;
using UnityCommander.Modules.FilePanel.Controllers;
using UnityCommander.Modules.FilePanel.States;

namespace UnityCommander.Modules.FilePanel.Layout
{
    public sealed class FilePanelLayoutController
    {
        private readonly FilePanelLayoutService _layoutService;
        private readonly Workspace _workspace;
        private readonly WorkspaceController _workspaceController;

        public FilePanelLayoutMode Mode { get; private set; }

        public LayoutNode CurrentLayout { get; private set; }

        public event Action? Changed;

        public FilePanelLayoutController(
            FilePanelLayoutService layoutService,
            Workspace workspace,
            WorkspaceController workspaceController,
            FilePanelLayoutMode initialMode = FilePanelLayoutMode.Split)
        {
            _layoutService = layoutService;
            _workspace = workspace;
            _workspaceController = workspaceController;

            Mode = initialMode;
            CurrentLayout = Build();
        }

        public void Initialize()
        {
            _workspaceController.Apply(Mode);
            CurrentLayout = Build();
        }

        public void SetMode(FilePanelLayoutMode mode)
        {
            if (Mode == mode)
                return;

            Mode = mode;

            _workspaceController.Apply(Mode);

            CurrentLayout = Build();

            Changed?.Invoke();
        }

        public void Toggle()
        {
            SetMode(
                Mode == FilePanelLayoutMode.Split
                    ? FilePanelLayoutMode.Unified
                    : FilePanelLayoutMode.Split);
        }

        private LayoutNode Build()
        {
            return _layoutService.Build(
                Mode,
                _workspace.HeaderRegion,
                _workspace.MainRegion,
                _workspace.SecondaryRegion);
        }
    }
}
