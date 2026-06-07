
using Prism.Commands;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.CommandSurface;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Modules.FilePanel.States;
using UnityCommander.Modules.FilePanel.States.Resolver;
using UnityCommander.Modules.FilePanel.ViewModels;
using UnityCommander.Services;

namespace UnityCommander.Modules.FilePanel.Controllers
{
    public class ContextMenuController
    {
        private readonly CommandSurfaceEngine _surface;
        private readonly CommandService _commandService;
        private readonly ContextResolverDispatcher _resolver;


        public ContextMenuController(
            CommandSurfaceEngine surface,
            CommandService commandService,
            ContextResolverDispatcher resolver)
        {
            _surface = surface;
            _commandService = commandService;
            _resolver = resolver;
        }

        public void Show(IContextMenuHost state, object? parameter)
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
