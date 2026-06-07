
using Prism.Commands;
using Prism.Dialogs;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Core;
using UnityCommander.Core.Mvvm;
using UnityCommander.Operation;
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
        private readonly CopyOperationController copyOperationController;
        private bool closeTrigger;
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
        public ICommand CopyCommand => new DelegateCommand(async () =>
        {
            this.CopyStateView = new CopyProcessView();

            if (manySource != null && manySource.Any())
            {
                // Запускаем одну общую операцию для всех источников
                await this.copyOperationController.StartCopyManyAsync(manySource, this.Target);
            }
            else
            {
                var source = this.Source;
                var dest = this.Target;
                await this.copyOperationController.StartCopyManyAsync(new[] { source }, dest);
            }
        });

        public ICommand MoveCommand => new DelegateCommand(() =>
        {
            //var cmdMove = this.globalCommandManager.GetCommand("Move");
            //cmdMove.Command.Execute(new object[] { this.Source, this.Target });
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
                this.manySource = copyParameters.ManySource;
            }
        }

        private void OnCopyCompleted()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (this.CloseDialogAfterCopyingComplete)
                {
                    RequestClose.Invoke(new DialogResult(ButtonResult.OK));
                }
            });
        }

        private void ExecuteCloseDialogCommand()
        {
            if (this.CloseDialogAfterCopyingComplete)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RequestClose.Invoke(new DialogResult(ButtonResult.OK));
                });
            }
        }
    }
}
