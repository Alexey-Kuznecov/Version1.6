
namespace UnityCommander.ViewModels
{
    using Prism.Commands;
    using Prism.Dialogs;
    using Prism.Mvvm;
    using System.Windows.Controls;
    using UnityCommander.Common.Dialog;
    using UnityCommander.Core.Mvvm;

    internal class DialogPluginConfigVm : BindableBase, IDialogAware
    {
        private DelegateCommand closeDialogCommand;

        private UserControl control;

        private IWindowManager _windowManager;

        public DialogPluginConfigVm(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        public DialogCloseListener RequestClose { get; private set; }

        public DelegateCommand CloseDialogCommand =>
            this.closeDialogCommand ??= new DelegateCommand(this.ExecuteCloseDialogCommand);

        public UserControl UserControl
        {
            get => this.control;
            set => this.SetProperty(ref this.control, value);
        }

        public string Title => "My Dialog";

        public bool CanCloseDialog()
        {
            return true;
        }
        public void OnDialogClosed()
        {
        }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            //_windowManager.ShowDialog("icon_maker-1.0");

            var param = parameters as OverrideDialogParameters;
            var type = param?.Package.GetType();
        }

        private void ExecuteCloseDialogCommand()
        {
            RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        }

        private void UnloadPlugin()
        {
        }
    }
}
