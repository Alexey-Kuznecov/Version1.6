
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
    public class CopyDialogViewModel : BindableBase, IDialogAware
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

        /// <summary>
        /// Описание смотри сдесь <see cref="CopyOnlyFolderContent"/>.
        /// </summary>
        private bool copyOnlyFolderContent;

        /// <summary>
        /// Описание смотри сдесь <see cref="СloseDialogAfterCopyingComplete"/>.
        /// </summary>
        private bool сloseDialogAfterCopyingComplete;

        #endregion
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyDialogViewModel"/> class.
        /// This the signature of the constructor needed for communication with another a view models.
        /// </summary>
        /// <param name="viewModelMessage"> Communication parameter of the view models. </param>
        public CopyDialogViewModel(IEventAggregator viewModelMessage, IGlobalCommandService globalCommandService)
        {
            this.globalCommandManager = globalCommandService.GetCommandManager();
            this.globalCommandManager.RegisterCommand("CloseCopyFileDialogCommand", this.CloseDialogCommand);
            this.viewModelMessage = viewModelMessage;
            this.CloseDialogAfterCopyingComplete = true;
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
        /// Gets the title.
        /// </summary>
        public string Title => "Копирование файлов";

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
        /// Данная опция отвечает за копирования содержимого папки. 
        /// Если опция равна истине то будет скопированно только содержимое 
        /// папки иначе будет скопированна вся папка. 
        /// </summary>
        public bool CopyOnlyFolderContent
        {
            get => this.copyOnlyFolderContent;
            set => this.SetProperty(ref this.copyOnlyFolderContent, value);
        }

        /// <summary>
        /// Данная опция отвечает за закрытие окна после копирования. 
        /// Если данная опция включена то после завершения копирования (файлов/попок),
        /// окно операции над файломи будет закрыто.
        /// </summary>
        public bool CloseDialogAfterCopyingComplete
        {
            get => this.сloseDialogAfterCopyingComplete;
            set => this.SetProperty(ref this.сloseDialogAfterCopyingComplete, value);
        }

        /// <summary>
        /// Gets the command to copy files or folders from one panel to another.
        /// </summary>
        public ICommand CopyCommand => new DelegateCommand(() =>
        {
            this.CopyStateView = new CopyProcessView();

            var source = new DirectoryInfo(this.Source);
            var destination = new DirectoryInfo(this.Target);

            if (!this.CopyOnlyFolderContent && source.Exists)
            {
                var fname = new DirectoryInfo(this.Source).Name;
                var dirC = destination.FullName + "\\" + fname;
                Directory.CreateDirectory(dirC);
                destination = new DirectoryInfo(dirC);
            }

            this.viewModelMessage.GetEvent<MessageSendEvent>().Publish(new object[] 
            {
                source,
                destination
            });
        });

        /// <summary>
        /// Gets the command to copy files or folders from one panel to another.
        /// </summary>
        public ICommand MoveCommand => new DelegateCommand(() =>
        {
            var cmdMove = this.globalCommandManager.GetCommand("Move");
            cmdMove.Command.Execute(new object[] { this.Source, this.Target });
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
            //if (Directory.Exists(Target))
            //{
            //    Directory.Delete(Target, true);
            //    Directory.CreateDirectory(Target);
            //}
        }

        /// <summary>
        /// The on dialog opened.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public void OnDialogOpened(IDialogParameters parameters)
        {
            this.CopyStateView = new CopyDialogControl();
            var param = parameters as OverrideDialogParameters;

            if (param?.Package is CopyParameters copyParameters)
            {
                this.Source = copyParameters.Source;
                this.Target = copyParameters.Target;
            }
        }

        /// <summary>
        /// The execute close dialog command.
        /// </summary>
        private void ExecuteCloseDialogCommand()
        {
            if (this.CloseDialogAfterCopyingComplete)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                });
            }
        }
    }
}
