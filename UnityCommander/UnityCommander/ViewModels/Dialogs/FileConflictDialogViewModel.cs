
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Windows.Input;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Abstractions.IO;
using UnityCommander.Operation;
namespace UnityCommander.ViewModels.Dialogs
{
    public sealed class FileConflictDialogViewModel : BindableBase, IDialogAware<FileConflictResult>
    {
        public string Title =>
            "Конфликт файлов";

        public FileConflict Conflict { get; private set; }

        public string SourcePath => Conflict?.SourcePath ?? string.Empty;
        public string DestinationPath => Conflict?.DestinationPath ?? string.Empty;

        public ICommand SkipCommand =>
            new DelegateCommand(() =>
            {
                Result = new FileConflictResult
                {
                    Action = FileConflictAction.Skip,
                    ApplyToAll = true
                };

                RequestClose?.Invoke();
            });


        public ICommand SkipAllCommand =>
            new DelegateCommand(() =>
            {
                Result = new FileConflictResult
                {
                    Action = FileConflictAction.SkipAll,
                    ApplyToAll = true
                };

                RequestClose?.Invoke();
            });

        public ICommand ReplaceAllCommand =>
            new DelegateCommand(() =>
            {
                Result = new FileConflictResult
                {
                    Action = FileConflictAction.ReplaceAll,
                    ApplyToAll = true
                };

                RequestClose?.Invoke();
            });

        public ICommand ReplaceCommand =>
            new DelegateCommand(() =>
            {
                Result = new FileConflictResult
                {
                    Action = FileConflictAction.Replace,
                    ApplyToAll = true
                };

                RequestClose?.Invoke();
            });

        public ICommand KeepBothCommand =>
            new DelegateCommand(() =>
            {
                Result = new FileConflictResult
                {
                    Action = FileConflictAction.KeepBoth,
                    ApplyToAll = true
                };

                RequestClose?.Invoke();
            });

        public ICommand CancelCommand =>
          new DelegateCommand(() =>
          {
              Result = new FileConflictResult
              {
                  Action = FileConflictAction.Cancel,
                  ApplyToAll = true
              };

              RequestClose?.Invoke();
          });

        public FileConflictResult Result { get; private set; }

        public Action RequestClose { get; set; }

        public void OnDialogOpened(object parameter)
        {
            if (parameter is FileConflict conflict)
            {
                Conflict = conflict;
            }
        }

        public void OnDialogClosed()
        {
        }

        public bool CanCloseDialog()
        {
            return true;
        }
    }
}
