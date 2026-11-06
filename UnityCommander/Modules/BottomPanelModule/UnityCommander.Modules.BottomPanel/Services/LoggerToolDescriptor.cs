
using System.Windows.Controls;
using UnityCommander.Logging.Core;
using UnityCommander.Modules.BottomPanel.ViewModels;
using UnityCommander.Modules.BottomPanel.Views;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Docking;

namespace UnityCommander.Modules.BottomPanel.Services
{
    public sealed class LoggerToolDescriptor : IToolDescriptor
    {
        private readonly LogViewModel _viewModel;

        public LoggerToolDescriptor(LogHub hub)
        {
            _viewModel = new LogViewModel(hub);
        }

        public string Id => "Logger";
        public string Title => "Logger";

        public bool CanCreateMultiple => false;

        public int MaxInstances => 1;

        public ToolDockSide DockSide 
            => ToolDockSide.Right;

        public string Icon => "logger";

        public string Description => "A logger for viewing and managing log entries.";

        public string Category => "Utilities";

        public Control Create()
        {
            return new LogView
            {
                DataContext = _viewModel
            };
        }
    }
}
