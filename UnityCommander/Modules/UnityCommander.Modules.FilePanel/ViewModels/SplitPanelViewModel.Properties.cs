
namespace UnityCommander.Modules.FilePanel.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Controls;

    using NLog;

    using Prism.Services.Dialogs;

    using UnityCommander.Common.Models.Directory;
    using UnityCommander.Common.Module;
    using UnityCommander.Core.Commands;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The split panel view model.
    /// </summary>
    public partial class SplitPanelViewModel
    {
        #region Declaration fields

        #region Dependencies Injection

        /// <summary>
        /// The dialog service.
        /// </summary>
        private readonly IDialogService dialogService;

        /// <summary>
        /// The directory provider.
        /// </summary>
        private readonly IDataProviderService dataService;

        /// <summary>
        /// The application settings.
        /// </summary>
        private readonly ISettings settingsService;

        /// <summary>
        /// The common state service.
        /// </summary>
        private readonly IMultiCommandService multiCommandService;

        /// <summary>
        /// The common state service.
        /// </summary>
        private readonly IGlobalCommandService globalCommandService;

        /// <summary>
        /// The plugin loader service.
        /// </summary>
        private readonly IPluginLoaderService pluginLoaderService;

        /// <summary>
        /// The config service.
        /// </summary>
        private readonly IAppConfigService configService;

        /// <summary>
        /// The command manager.
        /// </summary>
        private readonly CommandManager commandManager;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly Logger logger;

        /// <summary>
        /// If true, the plugin was cached and the result will be restored
        /// from the cache table the next time the program starts.
        /// </summary>
        private bool pluginValuesIsCached;

        /// <summary>
        /// Select base directory.
        /// </summary>
        private BaseDirectory selectedBaseDirectory;

        #endregion

        #region Collections

        /// <summary>
        /// The file list.
        /// </summary>
        private ObservableCollection<FileModel> fileList;

        /// <summary>
        /// The directory list.
        /// </summary>
        private ObservableCollection<FolderModel> directoryList;

        /// <summary>
        /// The drive list.
        /// </summary>
        private ObservableCollection<DriveModel> driveList;

        #endregion

        /// <summary>
        /// The navigation command.
        /// </summary>
        private NavigationInvoker navigationCommand;

        /// <summary>
        /// Control template for panel items.
        /// </summary>
        private ControlTemplate directoryPanelTemplate;

        /// <summary>
        /// Indicates the current directory.
        /// </summary>
        private string currentDirectory;

        #endregion

        /// <summary>
        /// Gets or sets the current directory.
        /// </summary>
        public string CurrentDirectory
        {
            get => this.currentDirectory;
            set => this.SetProperty(ref this.currentDirectory, value);
        }

        /// <summary>
        /// Gets or sets grid view for file panel.
        /// </summary>
        public ContextMenu ContextMenu { get; set; } = new ();

        /// <summary>
        /// Gets or sets grid view for file panel.
        /// </summary>
        public GridView FilePanelContainer { get; set; } = new ();

        /// <summary>
        /// Gets or sets grid view for folder panel.
        /// </summary>
        public GridView FolderPanelContainer { get; set; } = new ();

        /// <summary>
        /// Gets or sets grid view for folder panel.
        /// </summary>
        public GridView DrivePanelContainer { get; set; } = new ();

        /// <summary>
        /// Gets or sets the directory list.
        /// </summary>
        public ObservableCollection<FolderModel> DirectoryList
        {
            get => this.directoryList;
            set => this.SetProperty(ref this.directoryList, value);
        }

        /// <summary>
        /// Gets or sets the file list.
        /// </summary>
        public ObservableCollection<FileModel> FileList
        {
            get => this.fileList;
            set => this.SetProperty(ref this.fileList, value);
        }

        /// <summary>
        /// Gets or sets the file list.
        /// </summary>
        public ObservableCollection<DriveModel> DriveList
        {
            get => this.driveList;
            set => this.SetProperty(ref this.driveList, value);
        }


        /// <summary>
        /// Gets or sets the template for panel items.
        /// </summary>
        public ControlTemplate DirectoryPanelTemplate
        {
            get => this.directoryPanelTemplate;
            set => this.SetProperty(ref this.directoryPanelTemplate, value);
        }

        /// <summary>
        /// Sets the selected directory.
        /// </summary>
        public BaseDirectory SelectedBaseDirectory
        {
            set
            {
                if (value != null)
                {
                    this.SelectedDirectories.Add(value);
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the selected directory.
        /// </summary>
        public List<BaseDirectory> SelectedDirectories { get; set; } = new ();
    }
}
