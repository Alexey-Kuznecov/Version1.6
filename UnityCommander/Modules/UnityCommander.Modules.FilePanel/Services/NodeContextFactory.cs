
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using UnityCommander.Common.Commands;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Core.Navigation;
using UnityCommander.Logging;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Modules.FilePanel.Controllers;
using UnityCommander.Modules.FilePanel.Controllers.DnD;
using UnityCommander.Modules.FilePanel.States;
using UnityCommander.Services.Bootstrap;
using UnityCommander.Services.Interfaces;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.Services
{
    public class NodeContextFactory 
    {
        private readonly NavigationManager _navigation;
        private readonly ContextMenuController _menu;
        private readonly ISelectionManager _selection;
        private readonly ICommandUIService _commands;
        private readonly IDropTarget _dropTarget;
        private readonly NodeContextRegistry _contextRegistry;
        private ViewportMapper _scrollMapper;
        
        private readonly LoggerCreator _loggerCreator;
        private readonly ILogger _logger;

        public NodeContextFactory(
            NavigationManager navigation,
            ContextMenuController menu,
            ISelectionManager selection,
            ICommandUIService commands,
            GongDropAdapter dropTarget, 
            NodeContextRegistry nodeContext, 
            ViewportMapper scrollMapper)
        {
            _loggerCreator = Log.GetLoggerCreator();

            _logger = Log.Create("Navigation", LogScope.UserAction);

            _navigation = navigation;
            _menu = menu;
            _selection = selection;
            _commands = commands;
            _dropTarget = dropTarget;
            _contextRegistry = nodeContext;
            _scrollMapper = scrollMapper;
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

            var navFactory = new NavigationCommandFactory(_navigation, _selection, _commands);

            ctx.Commands.Add(
              navFactory.CreateGoBackCommand<FolderModel>(
                  CommandNames.Navigation.Back,
                  () => ctx.CanGoBack));

            ctx.Commands.Add(
                navFactory.CreateGoForwardCommand<FolderModel>(
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

            ctx.SelectionManager = _selection;

            return ctx;
        }
    }
}
