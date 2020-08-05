using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using UnityCommander.Core.IO;
using UnityCommander.Core;
using UnityCommander.Business;
using System.Windows;

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    public class CopyDialogViewModel : BindableBase
    {
        /// <summary>
        /// The path to the source panel.
        /// </summary>
        private string _source;
        /// <summary>
        /// The path to the target panel.
        /// </summary>
        private string _target;
        /// <summary>
        /// Gets or sets whether to ignore folders when copying.
        /// </summary>
        public bool FolderIgnore { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyDialogViewModel"/> class.
        /// </summary>
        public CopyDialogViewModel(IEventAggregator ea)
        {
            CopyCommand = new DelegateCommand(CopyExecute);
            ea.GetEvent<MessageSendEvent>().Subscribe(MessageReceived);
        }

        private void MessageReceived(string message)
        {
            MessageBox.Show(message);
        }

        /// <summary>
        /// Gets or sets the source panel.
        /// </summary>
        public string Source 
        { 
            get { return _source; }
            set { SetProperty(ref _source, value); }
        }
        /// <summary>
        /// Gets or sets the target panel.
        /// </summary>
        public string Target
        {
            get { return _target; }
            set { SetProperty(ref _target, value); }
        }
        /// <summary>
        /// Contains a list masks or templates for file extensions
        /// that will be included in copying.
        /// </summary>
        public List<string> ExtensionInclude { get; set; }
        /// <summary>
        /// Contains a list masks or templates for file extensions 
        /// that will be excluded of copying. 
        /// </summary>
        public List<string> ExtensionExclude { get; set; }
        /// <summary>
        /// Contains the command to coping files\folders.
        /// </summary>
        public ICommand CopyCommand { get; private set; }
        /// <summary>
        /// Copies files\folders from one panel to another.
        /// </summary>
        public void CopyExecute()
        {
            DirectoryItems directoryItems = new DirectoryItems();
            directoryItems.CopyFilesRecursively(
                new DirectoryInfo(Source), new DirectoryInfo(Target));
        }
    }
}
