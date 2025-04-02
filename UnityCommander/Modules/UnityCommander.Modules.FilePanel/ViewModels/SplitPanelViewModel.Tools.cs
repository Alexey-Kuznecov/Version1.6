
namespace UnityCommander.Modules.FilePanel.ViewModels
{
    using System;

    using Prism.Commands;

    using UnityCommander.Common.Models.Icons;
    using UnityCommander.Common.Module;
    using UnityCommander.Core.Commands;
    using UnityCommander.Core.Commands.Base;
    using UnityCommander.Integration.Commands;

    /// <summary>
    /// The split panel view model.
    /// </summary>
    public partial class SplitPanelViewModel
    {
        /// <summary>
        /// The computer icon.
        /// </summary>
        private IIcon thisComputerIcon;

        /// <summary>
        /// The computer icon.
        /// </summary>
        private IIcon backButtonIcon;

        /// <summary>
        /// The computer icon.
        /// </summary>
        private IIcon updateDirectoryPanelIcon;

        /// <summary>
        /// The computer icon.
        /// </summary>
        private bool thisComputerIconIsEnabled;

        /// <summary>
        /// The computer icon.
        /// </summary>
        private bool backButtonIsEnabled;

        /// <summary>
        /// The computer icon.
        /// </summary>
        private bool openFolderUnderCursorIsEnabled = true;

        /// <summary>
        /// Gets or sets a computer icon.
        /// </summary>
        public IIcon ThisComputerIcon
        {
            get => this.thisComputerIcon;
            set => this.SetProperty(ref this.thisComputerIcon, value);
        }

        /// <summary>
        /// Gets or sets a computer icon.
        /// </summary>
        public IIcon BackButtonIcon
        {
            get => this.backButtonIcon;
            set => this.SetProperty(ref this.backButtonIcon, value);
        }

        /// <summary>
        /// Gets or sets a computer icon.
        /// </summary>
        public IIcon UpdateDirectoryIcon
        {
            get => this.updateDirectoryPanelIcon;
            set => this.SetProperty(ref this.updateDirectoryPanelIcon, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this computer icon is enabled.
        /// </summary>
        public bool ThisComputerIconIsEnabled
        {
            get => this.thisComputerIconIsEnabled;
            set => this.SetProperty(ref this.thisComputerIconIsEnabled, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether back button is enabled.
        /// </summary>
        public bool BackButtonIsEnabled
        {
            get => this.backButtonIsEnabled;
            set => this.SetProperty(ref this.backButtonIsEnabled, value);
        }

        /// <summary>
        ///  Goes back to the previous directory.
        /// </summary>
        public DelegateCommand<object> GoBackDirectoryPanelCommand =>
            new DelegateCommand<object>(
                index =>
                    {
                        // this.currentTab.Content = TabContentFormat(this.currentPath);
                        if (this.navigationCommand.CanUndo)
                        {
                            this.navigationCommand.Previous();
                        }

                        if (!this.navigationCommand.CanUndo)
                        {
                            this.BackButtonIsEnabled = false;
                            this.ThisComputerIconIsEnabled = false;
                        }
                    });

        /// <summary>
        ///  Goes to the device and drive panel.
        /// </summary>
        public DelegateCommand<object> GoDrivePanelCommand => new DelegateCommand<object>(
            index =>
                {
                    var command = (ConcreteCommand)this.navigationCommand.FirstCommand;

                    if (command?.Receiver is Navigator navigator)
                    {
                        if (navigator.Path.Contains("Root:"))
                        {
                            navigator.CommandArg.Invoke(navigator.Path);
                        }
                    }

                    this.ThisComputerIconIsEnabled = false;
                });

        /// <summary>
        ///  Goes to the device and drive panel.
        /// </summary>
        public DelegateCommand<object> UpdateDirectoryPanelCommand => new DelegateCommand<object>(
            index =>
            {
                var globalCommandManager = globalCommandService.GetCommandManager();
                var cmd = globalCommandManager.GetCommand(CommandNames.DirectoryUpdate);
                cmd.Command?.Execute(null);
            });

        /// <summary>
        /// Goes back to the previous directory.
        /// </summary>
        /// <param name="obj">
        /// The command.
        /// </param>
        private void OnExecuteChanged(object obj)
        {
            if (obj is ConcreteCommand { Receiver: Navigator navigator })
            {
                if (!navigator.Path.Contains("Root:"))
                {
                    this.ThisComputerIconIsEnabled = true;
                    this.BackButtonIsEnabled = true;
                }
            }
        }
    }
}
