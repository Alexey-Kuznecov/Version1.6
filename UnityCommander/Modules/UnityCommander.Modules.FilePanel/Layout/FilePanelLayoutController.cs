
using System;
using UnityCommander.Controls.Layout;
using UnityCommander.Modules.FilePanel.States;

namespace UnityCommander.Modules.FilePanel.Layout
{
    public sealed class FilePanelLayoutController
    {
        private readonly FilePanelLayoutService _layoutService;
        private readonly Workspace _workspace;

        public FilePanelLayoutMode Mode { get; private set; }

        public LayoutNode CurrentLayout { get; private set; }

        public event Action<LayoutNode>? LayoutChanged;

        public FilePanelLayoutController(
            FilePanelLayoutService layoutService,
            Workspace workspace,
            FilePanelLayoutMode initialMode = FilePanelLayoutMode.Split)
        {
            _layoutService = layoutService;
            _workspace = workspace;

            Mode = initialMode;
            CurrentLayout = Build();
        }

        public void SetMode(FilePanelLayoutMode mode)
        {
            if (Mode == mode)
                return;

            Mode = mode;

            CurrentLayout = Build();

            LayoutChanged?.Invoke(CurrentLayout);
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
