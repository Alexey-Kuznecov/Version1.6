using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using UnityCommander.Integration.Plugins;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.ViewModels.Dialogs
{
    public class AppConfigDialogViewModel : BindableBase, IDialogAware
    {
        private IDialogService _dialogService; 

        public AppConfigDialogViewModel(IPluginLoaderService pluginService, IGlobalCommandService globalCommandService)
        {
            //_dialogService = dialogService;
        }

        public string Title => "AppConfig";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            //throw new NotImplementedException();
            return true;
        }

        public void OnDialogClosed()
        {
           // throw new NotImplementedException();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            //throw new NotImplementedException();
        }
    }
}
