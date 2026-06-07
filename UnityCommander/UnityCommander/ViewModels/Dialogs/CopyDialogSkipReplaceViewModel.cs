
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Prism.Commands;
using Prism.Dialogs;
using Prism.Events;
using Prism.Mvvm;
using UnityCommander.Core;
using UnityCommander.Core.IO.Operations;
using UnityCommander.Core.Mvvm;
namespace UnityCommander.ViewModels.Dialogs
{
    /// <summary>
    /// The dialog view model.
    /// </summary>
    public class CopyDialogSkipReplaceViewModel : BindableBase, IDialogAware
    {
        private readonly IEventAggregator viewModelMessage;

        private readonly MessageSendEvent messenger;

        private bool closeTrigger;

        private string source;

        private string target;

        public CopyDialogSkipReplaceViewModel(IEventAggregator viewModelMessage)
        {
            this.viewModelMessage = viewModelMessage;
        }

        public DelegateCommand CloseDialogCommand =>
            this.closeDialogCommand ??= new DelegateCommand(this.ExecuteCloseDialogCommand);

        private DelegateCommand closeDialogCommand;

        private UserControl control;

        private CopyInfo copyInfo;

        public string Title => "Диалог подтверждения замены файлов";

        public DialogCloseListener RequestClose { get; private set; }

        public UserControl CopyStateView
        {
            get => this.control;
            set => this.SetProperty(ref this.control, value);
        }

        public string Source
        {
            get => this.source;
            set => this.SetProperty(ref this.source, value);
        }

        public string Target
        {
            get => this.target;
            set => this.SetProperty(ref this.target, value);
        }

        public ICommand SkipAllCommand => new DelegateCommand(() =>
        {
            this.copyInfo.DialogSkipReplaceStatus = CopyDialogSkipReplaceStatus.SkipAll;
            this.viewModelMessage.GetEvent<MessageSendEvent>().Publish(this.copyInfo);
            //this.RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        });

        public ICommand ReplaceAllCommand => new DelegateCommand(() =>
        {
            this.copyInfo.DialogSkipReplaceStatus = CopyDialogSkipReplaceStatus.ReplaceAll;
            this.viewModelMessage.GetEvent<MessageSendEvent>().Publish(this.copyInfo);

            Application.Current.Dispatcher.Invoke(() =>
            {
                RequestClose.Invoke(new DialogResult(ButtonResult.OK));
            });
        });

        DialogCloseListener IDialogAware.RequestClose => throw new NotImplementedException();

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var param = parameters as OverrideDialogParameters;

            if (param?.Package is CopyInfo copyParameters)
            {
                this.copyInfo = copyParameters;
            }
        }

        private void ExecuteCloseDialogCommand()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                RequestClose.Invoke(new DialogResult(ButtonResult.OK));
            });
        }
    }
}
