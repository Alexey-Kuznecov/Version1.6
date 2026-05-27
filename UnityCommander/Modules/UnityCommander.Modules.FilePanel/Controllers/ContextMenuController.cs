
using Prism.Commands;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.CommandSurface;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Modules.FilePanel.States;
using UnityCommander.Modules.FilePanel.ViewModels;
using UnityCommander.Services;

namespace UnityCommander.Modules.FilePanel.Controllers
{
    public class ContextMenuController
    {
        private readonly CommandSurfaceEngine _surface;
        private readonly CommandService _commandService;
        private readonly FilePanelContextResolver _resolver;

        public ContextMenuController(
            CommandSurfaceEngine surface,
            CommandService commandService,
            FilePanelContextResolver resolver)
        {
            _surface = surface;
            _commandService = commandService;
            _resolver = resolver;
        }

        public void Show(PanelState state, object? parameter)
        {
            var ctx = _resolver.Resolve(state, parameter);

            var commands = _commandService
                .GetAll()
                .Select(x => x.Metadata)
                .ToList();

            var tree = _surface.Build(commands, ctx);

            var items = MapToMenu(tree);

            state.ContextMenuItems.Clear();
            foreach (var item in items)
                state.ContextMenuItems.Add(item);
        }

        private SurfaceContext BuildContext(PanelState state, object? parameter)
        {
            var ctx = _resolver.Resolve(state, parameter);

            var selected = state.SelectedItems?.Select(x => x.Path).ToList()
                           ?? new List<string>();

            if (selected.Count == 0 && parameter is BaseDirectory dir)
                selected.Add(dir.Path);

            ctx.Set(new FilePanelContext
            {
                CurrentPath = state.CurrentDirectory,
                SelectedFiles = selected
            });

            return ctx;
        }

        private List<MenuItemViewModel> MapToMenu(IEnumerable<SurfaceNode> nodes)
        {
            var result = new List<MenuItemViewModel>();

            foreach (var node in nodes)
            {
                var item = new MenuItemViewModel
                {
                    Title = node.CommandName == null
                        ? node.Title
                        : node.CommandName
                };

                if (node.CommandName != null)
                {
                    item.Command = new DelegateCommand(() =>
                    {
                        _commandService.ExecuteAsync(node.CommandName);
                    });
                }

                item.Children = MapToMenu(node.Children);
                result.Add(item);
            }

            return result;
        }
    }
}
