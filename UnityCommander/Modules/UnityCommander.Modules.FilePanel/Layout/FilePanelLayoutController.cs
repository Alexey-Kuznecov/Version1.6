
using System;
using System.Linq;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Common.Commands;
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

        public ViewMode CurrentViewMode { get; internal set; }

        public event Action<ViewMode>? ViewModeChanged;

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

        public void SetViewMode(ViewMode mode)
        {
            if (ApplyViewModeCore(mode))
                ViewModeChanged?.Invoke(mode);
        }

        internal void ApplyViewMode(ViewMode mode)
        {
            ApplyViewModeCore(mode);
        }

        private bool ApplyViewModeCore(ViewMode mode)
        {
            var content = _workspaceController.GetActiveContentNode();

            if (content is null || content.ViewMode == mode)
                return false;

            var headerNode = _workspaceController.GetHeaderContentNode();

            if (headerNode?.Context is NavigationNodeContext navigationContext)
            {
                var commands = navigationContext.Commands;

                var tableCommand = commands.FirstOrDefault(
                    x => x.Id == CommandNames.Views.Table);

                var thumbnailsCommand = commands.FirstOrDefault(
                    x => x.Id == CommandNames.Views.Thumbnails);

                if (tableCommand is not null)
                    tableCommand.IsChecked = mode == ViewMode.Table;

                if (thumbnailsCommand is not null)
                    thumbnailsCommand.IsChecked = mode == ViewMode.Thumbnails;
            }

            content.ViewMode = mode;

            CurrentLayout = Build();

            Changed?.Invoke(LayoutMode);

            return true;
        }
    }
}
