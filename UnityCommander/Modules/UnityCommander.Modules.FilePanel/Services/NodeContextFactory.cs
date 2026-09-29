
using Prism.Commands;
using System.Collections.ObjectModel;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Common.Commands;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Common.Panels;
using UnityCommander.Core.Navigation;
using UnityCommander.Logging;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Modules.FilePanel.Controllers;
using UnityCommander.Modules.FilePanel.Layout;
using UnityCommander.Modules.FilePanel.States;
using UnityCommander.Services.Interfaces;
using UnityCommander.WPF.DragDrop;
using UnityCommander.WPF.Overlays;

namespace UnityCommander.Modules.FilePanel.Services
{
    public class NodeContextFactory 
    {
        private readonly NavigationManager _navigation;
        private readonly ContextMenuController _menu;
        private readonly FilePanelLayoutController _layoutController;
        private readonly ISelectionManager _selection;
        private readonly ICommandUIService _commands;
        private readonly IDropTarget _dropTarget;
        private readonly NodeContextRegistry _contextRegistry;
        private readonly ILogger _logger;
        private readonly ActiveTabContext _activeTab;
        private readonly IPopupService _popupService;
        private readonly ICreationService _creationService;
        private ViewportMapper _scrollMapper;
        private readonly IWindowManager _windowManager;

        public NodeContextFactory(
            FilePanelLayoutController layoutController,
            ICreationService creationService,
            ContextMenuController menu,
            NavigationManager navigation,
            ISelectionManager selection,
            ICommandUIService commands,
            GongDropAdapter dropTarget, 
            NodeContextRegistry nodeContext, 
            ViewportMapper scrollMapper,
            IWindowManager windowManager,
            IPopupService popupService,
            ActiveTabContext activeTab)
        {
            var loggerCreator = Log.GetLoggerCreator();

            _logger = Log.Create("Navigation", LogScope.UserAction);
            
            _layoutController = layoutController;
            _activeTab = activeTab;
            _popupService = popupService;
            _navigation = navigation;
            _menu = menu;
            _selection = selection;
            _commands = commands;
            _dropTarget = dropTarget;
            _contextRegistry = nodeContext;
            _scrollMapper = scrollMapper;
            _creationService = creationService;
            _windowManager = windowManager;
        }

        public FolderNodeContext CreateFolderNode()
        {
            var ctx = new FolderNodeContext(
               _selection,
               _dropTarget,
               _menu,
               _navigation,
               _scrollMapper);

            _contextRegistry.Register(ctx);

            return ctx;
        }

        public FileNodeContext CreateFileNode()
        {
            var ctx = new FileNodeContext(
                _selection,
                _dropTarget,
                _menu,
                _scrollMapper);

            _contextRegistry.Register(ctx);

            return ctx;
        }

        public UnifiedNodeContext CreateUnifiedNode(
            FolderNodeContext folderNodeContext, 
            FileNodeContext fileNodeContext)
        {
            var ctx = new UnifiedNodeContext(
                _menu,
               folderNodeContext, 
               fileNodeContext);

            _contextRegistry.Register(ctx);

            return ctx;
        }

        public DriveNodeContext CreateDriveNode()
        {
            DriveNodeContext ctx = null;

            ctx = new DriveNodeContext()
            {
                NavigateCommand = new DelegateCommand<DriveModel>(dir =>
                {
                    if (dir != null)
                        _navigation.TryNavigateTo(dir.Letter);
#if (Nlog)
                    _logger.Info($"Переход в диск ({dir})");
#endif
                }),

                ShowContextMenuCommand = new DelegateCommand<object>(x =>
                {
                    _menu.Show(ctx, x);
                })
            };

            return ctx;
        }

        public NavigationNodeContext CreateHeaderNode()
        {
            var ctx = new NavigationNodeContext()
            {
                Navigation = _navigation,
                Commands = new ObservableCollection<UICommand>()
            };

            var navFactory = new NavigationCommandFactory(
                _layoutController, 
                _activeTab, 
                _popupService, 
                _creationService,
                _navigation, 
                _selection, 
                _commands, 
                _windowManager);

            ctx.Commands.Add(
               navFactory.CreateGoUpCommand(
                   CommandNames.Navigation.GoUp,
                   () => ctx.CanGoParent));

            ctx.Commands.Add(
              navFactory.CreateGoBackCommand<ActiveTabContext>(
                  CommandNames.Navigation.Back,
                  () => ctx.CanGoBack));

            ctx.Commands.Add(
                navFactory.CreateGoForwardCommand<ActiveTabContext>(
                  CommandNames.Navigation.Forward,
                  () => ctx.CanGoForward));

            ctx.Commands.Add(
                navFactory.CreateShowDrivesCommand(
                    CommandNames.Navigation.Drives,
                    () => true));

            ctx.Commands.Add(
                navFactory.CreateRefreshCommand(
                    CommandNames.Navigation.Refresh,
                    () => true));

            ctx.Commands.Add(
                 navFactory.CreateCreationFolderCommand(
                     CommandNames.Directory.Create,
                     () => true));

            ctx.Commands.Add(
                 navFactory.CreateCreationFileCommand(
                     CommandNames.File.Create,
                     () => true));

            ctx.Commands.Add(
                navFactory.CreateCreationCommand(
                    CommandNames.Panel.CreationItem,
                    () => true));

            ctx.Commands.Add(
             navFactory.CreateLayoutToggleCommand(
                 CommandNames.Views.Toggle,
                 () => true));

            ctx.Commands.Add(new UICommand());

            ctx.Commands.Add(
               navFactory.CreateSetThumbnailsModeCommand(
                   CommandNames.Views.Thumbnails,
                   () => true));

            ctx.Commands.Add(
                navFactory.CreateSetTableModeCommand(
                    CommandNames.Views.Table,
                    () => true));


            ctx.SelectionManager = _selection;

            return ctx;
        }
    }
}
