
namespace UnityCommander.Modules.FilePanel.ViewModels
{
    using System;
    using System.Windows.Controls;

    using Prism.Commands;
    using Prism.Regions;
    using UnityCommander.Common.Models.Icons;
    using UnityCommander.Core.Commands;
    using UnityCommander.Core.Mvvm;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The view a view model.
    /// </summary>
    public class ViewAViewModel : RegionViewModelBase
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
        /// The computer icon.
        /// </summary>
        private IIcon thisComputerIcon;

        /// <summary>
        /// The commandManager.
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
            this.commandManager = manager;
            this.ThisComputerIcon = iconProvider.GetIcon(MaterialDesignThemes.Wpf.PackIconKind.LaptopWindows);
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
        ///  Goes back to the previous directory.
        /// </summary>
        public DelegateCommand<object> NavigateBackDirLeftCommand =>
            new DelegateCommand<object>(
                index =>
        {
            this.navigationCommand ??= (NavigationInvoker)this.commandManager.GetCommand(Convert.ToInt32(index));

            if (this.navigationCommand.CanUndo)
                this.navigationCommand.Previous();
        });

        public DelegateCommand<object> GotoDrivePanel =>
            new DelegateCommand<object>(
                index =>
        {
            this.navigationCommand ??= (NavigationInvoker)this.commandManager.GetCommand(Convert.ToInt32(index));

            if (this.navigationCommand.CanUndo)
                this.navigationCommand.Previous();
        });

        /// <summary>
        ///  Goes back to the previous directory.
        /// </summary>
        public DelegateCommand<object> NavigateBackDirRightCommand =>
            new DelegateCommand<object>(
                index =>
        {
            this.navigationRCommand ??= (NavigationInvoker)this.commandManager.GetCommand(Convert.ToInt32(index));

            if (this.navigationRCommand.CanUndo)
                this.navigationRCommand.Previous();
        });
    }
}