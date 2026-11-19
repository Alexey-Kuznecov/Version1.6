
using System.Text.RegularExpressions;
using System.Windows.Controls;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Common.Panels;
using UnityCommander.Common.State;
using UnityCommander.Modules.BottomPanel.ViewModels;
using UnityCommander.Modules.BottomPanel.Views;
using UnityCommander.Preview.Abstraction;
using UnityCommander.Preview.Wpf;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Docking;

namespace UnityCommander.Modules.BottomPanel.Services
{
    public sealed class PreviewToolDescriptor : IToolDescriptor
    {
        private PreviewViewModel? _viewModel;

        private readonly VideoEngine _videoEngine;
        private readonly IPreviewService _previewService;
        private readonly IPreviewRegistry _previewRegistry;
        private readonly ISelectionService _selectionService;
        private readonly IPanelRegistry _panelRegistry;

        private readonly ActiveTabContext _activeTabContext;

        public string Id => "Preview";
        public string Title => "Preview";

        public bool CanCreateMultiple => false;
        public int MaxInstances => 1;

        public ToolDockSide DockSide => ToolDockSide.Center;

        public string Icon => "preview";

        public string Description => "Preview selected files.";

        public string Category => "Utilities";

        public PreviewToolDescriptor(
            IPreviewService previewService,
            IPreviewRegistry previewRegistry,
            ISelectionService selectionService,
            IPanelRegistry panelRegistry,
            ISessionAggregator aggregator,
            ActiveTabContext activeTabContext)
        {
            _previewService = previewService;
            _previewRegistry = previewRegistry;
            _selectionService = selectionService;
            _panelRegistry = panelRegistry;
            _activeTabContext = activeTabContext;

            aggregator.RegisterCapture(Capture);
            aggregator.RegisterRestore(Restore);
        }

        public Control Create()
        {
            var viewModel = new PreviewViewModel(
                _previewService,
                _previewRegistry,
                _selectionService,
                _panelRegistry,
                _activeTabContext);

            _viewModel = viewModel;

            return new PreviewView
            {
                DataContext = viewModel
            };
        }

        public void Capture(AppSessionState state)
        {
            _viewModel?.CaptureState(state.ExtensionStates);
        }

        public void Restore(AppSessionState state)
        {
            _viewModel?.RestoreState(state.ExtensionStates);
        }
    }
}
