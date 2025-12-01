
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Common.Commands;
using UnityCommander.Core;
using UnityCommander.Core.Mvvm;
using UnityCommander.Operation;
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

        private readonly IDirectoryChangeNotifier notifier;
        private readonly IGlobalCommandManager globalCommandManager;
        private readonly CopyOperationController copyOperationController;
        private bool closeTrigger;
        private string source;
        private string target;
        private bool copyOnlyFolderContent;
        private bool сloseDialogAfterCopyingComplete;
        private DelegateCommand closeDialogCommand;
        private UserControl control;
        public event Action<IDialogResult> RequestClose;

        #endregion

        public CopyDialogViewModel(IGlobalCommandService globalCommandService, CopyOperationController copyOperationController)
        {
            //this.notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
            this.globalCommandManager = globalCommandService.GetCommandManager();
            this.globalCommandManager.RegisterCommand("CloseCopyFileDialogCommand", this.CloseDialogCommand);
            this.copyOperationController = copyOperationController;
            this.CloseDialogAfterCopyingComplete = true;
            this.CopyOnlyFolderContent = false;
            //var filters = selectedTemplate switch
            //{
            //    "Только изображения" => new string[] { "*.jpg", "*.png", "*.bmp" },
            //    "Только документы" => new string[] { "*.docx", "*.pdf", "*.txt" },
            //    _ => new string[] { "*.*" }
            //};

            // Подписка на завершение копирования
            this.copyOperationController.Completed += OnCopyCompleted;
        }

        #region Свойства

        public string Title => "Копирование файлов";
        public DelegateCommand CloseDialogCommand => this.closeDialogCommand ??= new DelegateCommand(this.ExecuteCloseDialogCommand);
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

        #endregion

        #region Команды

        /// <summary>
        /// Gets the command to copy files or folders from one panel to another.
        /// </summary>
        public ICommand CopyCommand => new DelegateCommand(() =>
        {
            this.CopyStateView = new CopyProcessView();

            var source = new DirectoryInfo(this.Source);
            var destination = new DirectoryInfo(this.Target);

            this.copyOperationController.StartCopy(source.FullName, destination.FullName);
        });

        public ICommand MoveCommand => new DelegateCommand(() =>
        {
            var cmdMove = this.globalCommandManager.GetCommand("Move");
            cmdMove.Command.Execute(new object[] { this.Source, this.Target });
        });

        #endregion

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {  
            // Отписываемся от события
            this.copyOperationController.Completed -= OnCopyCompleted;
        }

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

        private void OnCopyCompleted()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (this.CloseDialogAfterCopyingComplete)
                {
                    this.RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                }
            });
        }

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
