using Prism.Dialogs;
using Prism.Mvvm;
using System;

namespace UnityCommander.ViewModels.Dialogs
{
    public class AppConfigDialogViewModel : BindableBase, IDialogAware
    {
        private IDialogService _dialogService; 

        public AppConfigDialogViewModel()
        {
            //_dialogService = dialogService;
        }

        public string Title => "AppConfig";

        public DialogCloseListener RequestClose { get; private set; }

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
