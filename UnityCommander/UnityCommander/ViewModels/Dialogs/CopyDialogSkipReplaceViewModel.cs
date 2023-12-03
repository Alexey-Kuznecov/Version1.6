
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using UnityCommander.Common.Commands;
using UnityCommander.Core;
using UnityCommander.Core.IO.Operations;
using UnityCommander.Core.Mvvm;
using UnityCommander.Integration.Commands;
using UnityCommander.Services.Interfaces;
using UnityCommander.Views.CopyDialogs;
using UnityCommander.Views.Dialogs;

namespace UnityCommander.ViewModels.Dialogs
{
    /// <summary>
    /// The dialog view model.
    /// </summary>
    public class CopyDialogSkipReplaceViewModel : BindableBase, IDialogAware
    {
        
        #region Declaration Fields

        /// <summary>
        /// The view model message.
        /// </summary>
        private readonly IEventAggregator viewModelMessage;

        /// <summary>
        /// Содержит ссылку на менеджер для регистрации или выполнения глобальных команд.
        /// </summary>
        private readonly IGlobalCommandManager globalCommandManager;

        /// <summary>
        /// 
        /// </summary>
        private readonly MessageSendEvent messenger;

        /// <summary>
        /// The close trigger.
        /// </summary>
        private bool closeTrigger;

        /// <summary>
        /// Contains the path to the source panel.
        /// </summary>
        private string source;

        /// <summary>
        /// Contains the path to the target panel.
        /// </summary>
        private string target;

        #endregion
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyDialogViewModel"/> class.
        /// This the signature of the constructor needed for communication with another a view models.
        /// </summary>
        /// <param name="viewModelMessage"> Communication parameter of the view models. </param>
        public CopyDialogSkipReplaceViewModel(IEventAggregator viewModelMessage, IGlobalCommandService globalCommandService)
        {
            this.globalCommandManager = globalCommandService.GetCommandManager();
            //messenger = viewModelMessage.GetEvent<MessageSendEvent>();
            //messenger.Subscribe(this.SetupCopyFiles);
            //this.globalCommandManager.RegisterCommand("CloseCopyFileDialogCommand", this.CloseDialogCommand);
            this.viewModelMessage = viewModelMessage;
        }

        /// <summary>
        /// The close dialog command.
        /// </summary>
        public DelegateCommand CloseDialogCommand =>
            this.closeDialogCommand ??= new DelegateCommand(this.ExecuteCloseDialogCommand);

        /// <summary>
        /// The close dialog command.
        /// </summary>
        private DelegateCommand closeDialogCommand;

        /// <summary>
        /// The control.
        /// </summary>
        private UserControl control;


        /// <summary>
        /// The control.
        /// </summary>
        private CopyInfo copyInfo;

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title => "Диалог подтверждения замены файлов";

        /// <summary>
        /// The request close.
        /// </summary>
        public event Action<IDialogResult> RequestClose;

        /// <summary>
        /// Gets or sets the user control.
        /// </summary>
        public UserControl CopyStateView
        {
            get => this.control;
            set => this.SetProperty(ref this.control, value);
        }

        /// <summary>
        /// Gets or sets the source panel.
        /// </summary>
        public string Source
        {
            get => this.source;
            set => this.SetProperty(ref this.source, value);
        }

        /// <summary>
        /// Gets or sets the target panel.
        /// </summary>
        public string Target
        {
            get => this.target;
            set => this.SetProperty(ref this.target, value);
        }

        /// <summary>
        /// Gets the command to copy files or folders from one panel to another.
        /// </summary>
        public ICommand SkipAllCommand => new DelegateCommand(() =>
        {
            this.copyInfo.DialogSkipReplaceStatus = CopyDialogSkipReplaceStatus.SkipAll;
            this.viewModelMessage.GetEvent<MessageSendEvent>().Publish(this.copyInfo);
            //this.RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        });

        /// <summary>
        /// Gets the command to copy files or folders from one panel to another.
        /// </summary>
        public ICommand ReplaceAllCommand => new DelegateCommand(() =>
        {
            this.copyInfo.DialogSkipReplaceStatus = CopyDialogSkipReplaceStatus.ReplaceAll;
            this.viewModelMessage.GetEvent<MessageSendEvent>().Publish(this.copyInfo);

            Application.Current.Dispatcher.Invoke(() =>
            {
                this.RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            });
        });

        /// <summary>
        /// The can close dialog.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanCloseDialog()
        {
            return true;
        }

        /// <summary>
        /// The on dialog closed.
        /// </summary>
        public void OnDialogClosed()
        {
        }

        /// <summary>
        /// The on dialog opened.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public void OnDialogOpened(IDialogParameters parameters)
        {
            var param = parameters as OverrideDialogParameters;

            if (param?.Package is CopyInfo copyParameters)
            {
                this.copyInfo = copyParameters;
            }
        }

        /// <summary>
        /// The execute close dialog command.
        /// </summary>
        private void ExecuteCloseDialogCommand()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            });
        }
    }
}
