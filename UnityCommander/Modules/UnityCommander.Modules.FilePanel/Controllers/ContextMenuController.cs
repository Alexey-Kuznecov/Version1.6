
using CommandSystem.Abstractions;
using Prism.Commands;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.CommandSurface;
using UnityCommander.Modules.FilePanel.States.Resolver;
using UnityCommander.Modules.FilePanel.ViewModels;
using UnityCommander.Services;

namespace UnityCommander.Modules.FilePanel.Controllers
{
    public class ContextMenuController
    {
        private readonly CommandSurfaceEngine _surface;
        private readonly CommandExecutionService _commandExecution;
        private readonly CommandRegistryService _commandRegistry;
        private readonly ContextResolverDispatcher _resolver;


        public ContextMenuController(
            CommandSurfaceEngine surface,
            CommandExecutionService commandExecution,
            CommandRegistryService commandRegistry,
            ContextResolverDispatcher resolver)
        {
            _surface = surface;
            _commandExecution = commandExecution;
            _commandRegistry = commandRegistry;
            _resolver = resolver;
        }

        public void Show(IContextMenuHost state, object? parameter)
        {
            var ctx = _resolver.Resolve(state, parameter);

            var commands = _commandRegistry
                .GetAll()
                .Select(x => x.Metadata)
                .ToList();

            var tree = _surface.Build(commands, ctx);

            var items = MapToMenu(tree, ctx);

            state.ContextMenuItems.Clear();
            foreach (var item in items)
                state.ContextMenuItems.Add(item);
        }

        private List<MenuItemViewModel> MapToMenu(
            IEnumerable<SurfaceNode> nodes, 
            SurfaceContext context)
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
                        var menuContext = context.Get<FilePanelContextMenu>();

                        var ctx = new CommandContext(node.CommandName, menuContext, menuContext?.SelectedPaths);

                        _commandExecution.ExecuteAsync(node.CommandName, ctx);
                    });
                }

                item.Children = MapToMenu(node.Children, context);
                result.Add(item);
            }

            return result;
        }
    }
}
