
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
    using UnityCommander.Core.Behaviors;
    using UnityCommander.Modules.FilePanel.Controllers;
    using UnityCommander.Modules.FilePanel.Controllers.DnD;
    using UnityCommander.Modules.FilePanel.States.Resolver;
    using UnityCommander.Modules.FilePanel.Views;
    using UnityCommander.Services.Bootstrap;
    using UnityCommander.Services.Interfaces;

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
            
            // Контестное меню 
            containerRegistry.RegisterSingleton<IContextMenuResolver, DriveContextMenuResolver>();
            containerRegistry.RegisterSingleton<IContextMenuResolver, FilePanelContextMenuResolver>();
            containerRegistry.RegisterSingleton<ContextResolverDispatcher>();
            containerRegistry.RegisterSingleton<ContextMenuController>();

            // DragDrop
            containerRegistry.RegisterSingleton<IDropContextResolver, NodeDragDropContextResolver>();
            containerRegistry.RegisterSingleton<IDragDropHandler, FilePanelDragDropHandler>();
            containerRegistry.RegisterSingleton<IDragDropVisualService, DragDropVisualService>();
            containerRegistry.RegisterSingleton<DragDropController>();
            containerRegistry.RegisterSingleton<DragDropContextFactory>();
            containerRegistry.RegisterSingleton<GongDropAdapter>();


        }   
    }
}