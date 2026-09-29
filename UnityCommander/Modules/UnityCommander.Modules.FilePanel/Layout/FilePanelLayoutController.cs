
using System;
using UnityCommander.Common.Models;
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

        public FilePanelLayoutMode LayoutMode { get; private set; }

        public LayoutNode CurrentLayout { get; private set; }

        public event Action<FilePanelLayoutMode>? Changed;

        public FilePanelLayoutController(
            FilePanelLayoutService layoutService,
            Workspace workspace,
            WorkspaceController workspaceController,
            FilePanelLayoutMode initialMode = FilePanelLayoutMode.Unified)
        {
            _layoutService = layoutService;
            _workspace = workspace;
            _workspaceController = workspaceController;

            LayoutMode = initialMode;
            CurrentLayout = Build();
        }

        public void Initialize()
        {
            _workspaceController.Apply(LayoutMode);
            CurrentLayout = Build();
        }

        public void SetLayoutMode(FilePanelLayoutMode mode)
        {
            if (LayoutMode == mode)
                return;

            LayoutMode = mode;

            _workspaceController.Apply(LayoutMode);

            CurrentLayout = Build();

            Changed?.Invoke(mode);
        }

        public void SetViewMode(ViewMode mode)
        {
            var content = _workspaceController.GetActiveContentNode();

            if (content is null)
                return;

            content.ViewMode = mode;

            //_workspaceController.Apply(LayoutMode);

            CurrentLayout = Build();

            Changed?.Invoke(LayoutMode);
        }

        public void Toggle()
        {
            SetLayoutMode(
                LayoutMode == FilePanelLayoutMode.Split
                    ? FilePanelLayoutMode.Unified
                    : FilePanelLayoutMode.Split);
        }

        private LayoutNode Build()
        {
            return _layoutService.Build(
                LayoutMode,
                _workspace.HeaderRegion,
                _workspace.MainRegion,
                _workspace.SecondaryRegion);
        }

        internal void ShowDrives()
        {
            //SetMode(FilePanelLayoutMode.Split);
            _workspaceController.ShowMyComputerMode();
        }
    }
}
