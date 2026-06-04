
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Text.RegularExpressions;
using UnityCommander.Common.State;
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
            
            SaveStateCommand =
                 new DelegateCommand<AppSessionState>(Capture);
        }

        public DelegateCommand<AppSessionState> SaveStateCommand { get; }

        internal void Capture(AppSessionState state)
        {
        }

        internal void Restore(AppSessionState state)
        {
            //throw new NotImplementedException();
        }
    }
}
 