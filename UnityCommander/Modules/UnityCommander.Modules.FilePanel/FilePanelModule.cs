
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
    using Prism.Ioc;
    
    using Prism.Modularity;
    using Prism.Navigation.Regions;
    using UnityCommander.Core.Behaviors;
    using UnityCommander.Modules.FilePanel.Controllers;
    using UnityCommander.Modules.FilePanel.Controllers.DnD;
    using UnityCommander.Modules.FilePanel.States.Resolver;
    using UnityCommander.Modules.FilePanel.Views;

    public class FilePanelModule : IModule
    {
        private readonly IRegionManager regionManager;

        public FilePanelModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
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