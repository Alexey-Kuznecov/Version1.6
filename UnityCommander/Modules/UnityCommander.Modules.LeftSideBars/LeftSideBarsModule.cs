
using System;

namespace UnityCommander.Modules.LeftSideBars
{
    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Navigation.Regions;
    using UnityCommander.Common;
    using UnityCommander.Common.Sidebar;
    using UnityCommander.Core;
    using UnityCommander.Modules.LeftSideBars.Content;
    using UnityCommander.Modules.LeftSideBars.SidebarContent;
    using UnityCommander.Modules.LeftSideBars.ViewModels;
    using UnityCommander.Modules.LeftSideBars.Views;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Interfaces.Sidebar;

    public class LeftSideBarsModule : IModule
    {
        private readonly IRegionManager regionManager;

        public LeftSideBarsModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            this.regionManager.RequestNavigate(RegionNames.LeftSideBarRegion, nameof(Sidebar));
           
            var sidebarService = containerProvider.Resolve<SidebarService>();
            var coordinator =
                containerProvider.Resolve<ISessionAggregator>();

            var vm = RegionViewModelHelper.GetViewModel<SidebarViewModel>(
                regionManager,
                RegionNames.LeftSideBarRegion);

            if (vm != null)
            {
                coordinator.RegisterCapture(vm.Capture);
                coordinator.RegisterRestore(vm.Restore);
            }

            sidebarService.Register(
                new SidebarSection(
                    "core.column",
                    "TableColumn",
                    new ColumnsOptionControl(),
                    new ColumnOptionViewModel()
                ));

            sidebarService.Register(
                new SidebarSection
                (
                   "core.plugins",
                   "Plugin",
                    new PluginControlPanel(),
                    new PluginPanelViewModel()
                ));

            sidebarService.Register(
               new SidebarSection
               (
                  "core.commnet",
                  "Comment",
                   new CommentControl(),
                   null
               ));

            sidebarService.Register(
               new SidebarSection
               (
                  "core.foldertree",
                  "FileTree",
                   new FolderTreeOverviewControl(),
                   null
               ));

            sidebarService.Register(
               new SidebarSection
               (
                  "core.tag",
                  "Tag",
                  new TagControlPanel(),
                   null
               ));

            vm.Initialize();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Sidebar>();
        }
    }
}