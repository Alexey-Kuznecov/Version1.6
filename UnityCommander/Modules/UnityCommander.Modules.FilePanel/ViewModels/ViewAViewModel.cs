
namespace UnityCommander.Modules.FilePanel.ViewModels
{
    using Prism.Commands;
    using Prism.Regions;
    using System;
    using System.Diagnostics.CodeAnalysis;

    using UnityCommander.Common.Models.Icons;
    using UnityCommander.Core.Commands;
    using UnityCommander.Core.Commands.Base;
    using UnityCommander.Core.Modules;
    using UnityCommander.Core.Mvvm;
    using UnityCommander.Modules.FilePanel.Views;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The view a view model.
    /// </summary>
    [SuppressMessage("ReSharper", "StyleCop.SA1503")]
    public class ViewAViewModel : RegionViewModelBase, IPanelContainer
    {
        /// <summary>
        /// The navigationCommand class instance.
        /// </summary>
        private NavigationInvoker navigationCommand;

        /// <summary>
        /// The navigationCommand class instance.
        /// </summary>
        private NavigationInvoker navigationRCommand;

        /// <summary>
        /// The region manager.
        /// </summary>
        private IRegionManager regionManager;

        /// <summary>
        /// The computer icon.
        /// </summary>
        private IIcon thisComputerIcon;

        /// <summary>
        /// The computer icon.
        /// </summary>
        private bool thisComputerIconIsEnabled;

        /// <summary>
        /// The computer icon.
        /// </summary>
        private bool backButtonIsEnabled;

        /// <summary>
        /// The command manager.
        /// </summary>
        private CommandManager commandManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAViewModel"/> class.
        /// </summary>
        /// <param name="regionManager">
        /// The region commandManager.
        /// </param>
        /// <param name="iconProvider">
        /// The icon Provider.
        /// </param>
        /// <param name="manager">
        /// The commandManager.
        /// </param>
        public ViewAViewModel(IRegionManager regionManager, IIconProviderService iconProvider, CommandManager manager)
            : base(regionManager)
        {
            this.regionManager = regionManager;
            this.commandManager = manager;
            this.ThisComputerIcon = iconProvider.GetIcon(MaterialDesignThemes.Wpf.PackIconKind.LaptopWindows);
            this.ThisComputerIconIsEnabled = true;
            this.BackButtonIsEnabled = true;
        }

        /// <summary>
        /// Gets or sets a computer icon.
        /// </summary>
        public IIcon ThisComputerIcon
        {
            get => this.thisComputerIcon;
            set => this.SetProperty(ref this.thisComputerIcon, value);
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
        /// Gets or sets the token.
        /// </summary>
        public Guid Token { get; set; }

        /// <summary>
        ///  Goes back to the previous directory.
        /// </summary>
        public DelegateCommand<object> GoBackDirectoryLeftPanelCommand =>
            new DelegateCommand<object>(
                index =>
        {
            if (this.navigationCommand.CanUndo)
                this.navigationCommand.Previous();

            if (!this.navigationCommand.CanUndo)
            {
                this.navigationCommand.OnExecuteChanged += OnExecuteChanged;
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

                    this.navigationCommand.OnExecuteChanged += OnExecuteChanged;
                    this.ThisComputerIconIsEnabled = false;
                });

        /// <summary>
        ///  Goes back to the previous directory.
        /// </summary>
        public DelegateCommand<object> GoBackDirectoryRightPanelCommand =>
            new DelegateCommand<object>(
                index =>
                    {
                        if (this.navigationRCommand.CanUndo)
                            this.navigationRCommand.Previous();

                        if (!this.navigationRCommand.CanUndo)
                        {
                            this.navigationRCommand.OnExecuteChanged += OnExecuteChanged;
                            this.BackButtonIsEnabled = false;
                            this.ThisComputerIconIsEnabled = false;
                        }
                    });

        /// <summary>
        /// The initial panel.
        /// </summary>
        /// <param name="panelToken">
        /// The panel token.
        /// </param>
        public void InitialPanel(Guid[] panelToken)
        {
            this.navigationCommand ??= (NavigationInvoker)this.commandManager.GetCommand(panelToken[0]);
            this.navigationRCommand ??= (NavigationInvoker)this.commandManager.GetCommand(panelToken[1]);
        }

        /// <summary>
        /// Goes back to the previous directory.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        private void OnExecuteChanged(ConcreteCommand command)
        {
            if (command.Receiver is Navigator navigator)
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