
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Common.Override.Engine;
using UnityCommander.Operation;
using UnityCommander.Views.CopyDialogs;

namespace UnityCommander.ViewModels.Dialogs
{
    /// <summary>
    /// The dialog view model.
    /// </summary>
    public class CopyDialogViewModel : BindableBase, IDialogAware<CopyDialogResult>
    {
        
        #region Declaration Fields

        private readonly CopyOperationController copyOperationController;
        private string source;
        private string target;
        public List<string> manySource;
        private bool copyOnlyFolderContent;
        private bool сloseDialogAfterCopyingComplete;
        private DelegateCommand closeDialogCommand;
        private UserControl control;
        
        #endregion

        public CopyDialogViewModel(CopyOperationController copyOperationController)
        {
            this.copyOperationController = copyOperationController;
            this.CloseDialogAfterCopyingComplete = true;
            this.CopyOnlyFolderContent = false;
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

        public bool CopyOnlyFolderContent
        {
            get => this.copyOnlyFolderContent;
            set => this.SetProperty(ref this.copyOnlyFolderContent, value);
        }

        public bool CloseDialogAfterCopyingComplete
        {
            get => this.сloseDialogAfterCopyingComplete;
            set => this.SetProperty(ref this.сloseDialogAfterCopyingComplete, value);
        }

        public ICommand CopyCommand => new DelegateCommand(async () =>
        {
            Result = new CopyDialogResult
            {
                Accepted = true,
                Request = BuildRequest()
            };

            RequestClose?.Invoke();
        });

        private FileOperationRequest BuildRequest()
        {
            var request = new FileOperationRequest();

            foreach (var source in this.manySource)
            {
                request.Sources.Add(source);
            }

            request.Target = this.Target;

            return request;
        }

        public ICommand MoveCommand => new DelegateCommand(() =>
        {
            //var cmdMove = this.globalCommandManager.GetCommand("Move");
            //cmdMove.Command.Execute(new object[] { this.Source, this.Target });
        });

        public CopyDialogResult Result { get; set; }

        public Action RequestClose { get ; set ; }

        #endregion

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {  
            this.copyOperationController.Completed -= OnCopyCompleted;
        }

        public void OnDialogOpened(object? parameters)
        {
            if (parameters is FileOperationRequest request)
            {
                this.CopyStateView = new CopyDialogControl();
                this.Source = request.Sources[0];
                this.Target = request.Target;
                this.manySource = request.Sources;
            }
        }

        private void OnCopyCompleted(CopyOperationResult copyOperation)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (this.CloseDialogAfterCopyingComplete)
                {
                    RequestClose?.Invoke();
                }
            });
        }

        private void ExecuteCloseDialogCommand()
        {
            if (this.CloseDialogAfterCopyingComplete)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RequestClose?.Invoke();
                });
            }
        }
    }
}
