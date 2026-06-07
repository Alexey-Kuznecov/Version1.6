
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UnityCommander.Common.Commands;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Core.DragDrop;
using UnityCommander.Core.Navigation;
using UnityCommander.Modules.FilePanel.Controllers;
using UnityCommander.Modules.FilePanel.Controllers.DnD;
using UnityCommander.Modules.FilePanel.States;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.FilePanel.Services
{
    public class NodeContextFactory 
    {
        private readonly NavigationManager _navigation;
        private readonly ContextMenuController _menu;
        private readonly ISelectionManager _selection;
        private readonly ICommandUIService _commands;
        private readonly IDropTarget _dropTarget;

        public NodeContextFactory(
            NavigationManager navigation,
            ContextMenuController menu,
            ISelectionManager selection,
            ICommandUIService commands,
            GongDropAdapter dropTarget)
        {
            _navigation = navigation;
            _menu = menu;
            _selection = selection;
            _commands = commands;
            _dropTarget = dropTarget;
        }

        public FolderNodeContext CreateFolderNode()
        {
            FolderNodeContext ctx = null;

            ctx = new FolderNodeContext()
            {
                SelectionManager = _selection,

                NavigateCommand = new DelegateCommand<FolderModel>(dir =>
                {
                    var sw = Stopwatch.StartNew();
                 
                    if (dir != null)
                        _navigation.TryNavigateTo(dir.Path);
                    sw.Stop();

                    Debug.WriteLine($"NavigateTo: {sw.ElapsedMilliseconds} ms");
                }),

                ShowContextMenuCommand = new DelegateCommand<object>(x =>
                {
                    _menu.Show(ctx, x);
                }),

                DropTarget = _dropTarget
            };

            return ctx;
        }

        public FileNodeContext CreateFileNode()
        {
            FileNodeContext ctx = null;

            ctx = new FileNodeContext()
            {
                SelectionManager = _selection,

                ShowContextMenuCommand = new DelegateCommand<object>(x =>
                {
                    _menu.Show(ctx, x);
                }),

                DropTarget = _dropTarget
            };

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
