using Prism.Commands;
using Prism.Mvvm;
using UnityCommander.Services;

namespace UnityCommander.Modules.BottomPanel.ViewModels
{
    public class BottomPanelViewModel : BindableBase
    {
        private string _message;
        private LoggingSinkService _loggingService;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public BottomPanelViewModel(LoggingSinkService loggingService)
        {
            _loggingService = loggingService;
            Message = "View A from your Prism Module";
        }
    }
}
