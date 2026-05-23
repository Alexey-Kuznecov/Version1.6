
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilePanelModule.cs" company="T">
// Copyright (p) Alexey Kuznecov. All right reserved.
// </copyright>
// <summary>
//   The file panel module.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityCommander.Modules.FilePanel
{
    using Prism.Commands;
    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Regions;
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using UnityCommander.Common.Module;
    using UnityCommander.Logging.Contracts;
    using UnityCommander.Modules.FilePanel.Views;
    using UnityCommander.Services.Bootstrap;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Interfaces.Database.Queries.Xml;
    using UnityCommander.Services.Interfaces.Settings;
    using Xceed.Wpf.AvalonDock;
    using Xceed.Wpf.AvalonDock.Layout;
    using Xceed.Wpf.AvalonDock.Layout.Serialization;

    /// <summary>
    /// The file panel module.
    /// </summary>
    public class FilePanelModule : IModule
    {
        /// <summary>
        /// The _region manager.
        /// </summary>
        private readonly IRegionManager regionManager;
        private IMultiCommandService  _multiCommands;

        public FilePanelModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _multiCommands = containerProvider.Resolve<IMultiCommandService>();
            var initializer = containerProvider.Resolve<AppInitializer>();
            _multiCommands.SaveCommand.RegisterCommand(initializer.SavePanelStateCommand);
            initializer.Initialize();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SplitPanelView>();
        }
    }
}