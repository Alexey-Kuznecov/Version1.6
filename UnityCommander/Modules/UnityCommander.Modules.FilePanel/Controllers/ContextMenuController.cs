
using CommandSystem.Abstractions;
using Prism.Commands;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.CommandSurface;
using UnityCommander.Common.Commands;
using UnityCommander.Modules.FilePanel.States.Resolver;
using UnityCommander.Modules.FilePanel.States.Resolver.Models;
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
        private readonly ISurfaceCategoryPresentation _categoryPresentation;

        public ContextMenuController(
            CommandSurfaceEngine surface,
            CommandExecutionService commandExecution,
            CommandRegistryService commandRegistry,
            ContextResolverDispatcher resolver,
            ISurfaceCategoryPresentation categoryPresentation)
        {
            _surface = surface;
            _commandExecution = commandExecution;
            _commandRegistry = commandRegistry;
            _resolver = resolver;
            _categoryPresentation = categoryPresentation;
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
                if (node.CommandName is not null)
                {
                    result.Add(CreateCommandItem(node, context));
                    continue;
                }

                var presentation = _categoryPresentation.Get(node.Title);

                var children = MapToMenu(node.Children, context);

                if (children.Count == 0)
                    continue;

                if (presentation == SurfaceCategoryPresentation.Separator)
                {
                    if (result.Count > 0)
                    {
                        result.Add(new MenuItemViewModel
                        {
                            IsSeparator = true
                        });
                    }

                    result.AddRange(children);
                    continue;
                }

                result.Add(new MenuItemViewModel
                {
                    Title = node.Title,
                    Children = children
                });
            }

            return result;
        }

        private MenuItemViewModel CreateCommandItem(
            SurfaceNode node,
            SurfaceContext context)
        {
            var name = CommandPresentationProvider
                .Get(node.CommandName!)
                .DisplayName;

            var item = new MenuItemViewModel
            {
                Title = name ?? node.CommandName!,
                IconKey = node.CommandName!
            };

            item.Command = new DelegateCommand(() =>
            {
                _commandExecution.ExecuteAsync(node.CommandName!);
            });

            return item;
        }
    }
}
