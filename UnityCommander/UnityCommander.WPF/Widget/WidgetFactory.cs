
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Linq;
using UnityCommander.Abstractions.Widget;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.WPF.Widget
{
    public sealed class WidgetFactory : IWidgetFactory
    {
        private readonly IWidgetRegistry _registry;
        private readonly IServiceScopeResolver _scopeResolver;

        public WidgetFactory(
            IWidgetRegistry registry,
            IServiceScopeResolver scopeResolver)
        {
            _registry = registry;
            _scopeResolver = scopeResolver;
        }

        public WidgetItem Create(string widgetId, int order)
        {
            var definition = _registry.Get(widgetId);
            var provider = _scopeResolver.Resolve("");

            var viewModel = ActivatorUtilities.CreateInstance(
                provider,
                definition.ViewModelType);

            var view = (FrameworkElement)
                Activator.CreateInstance(definition.ViewType)!;

            view.DataContext = viewModel;

            return new WidgetItem
            {
                Id = widgetId,
                Title = definition.Name,
                View = view,
                Order = order,

                PrimaryAction = definition.PrimaryAction is null
                    ? null
                    : CreateAction(definition.PrimaryAction, viewModel),

                Actions = definition.Actions?
                    .Select(x => CreateAction(x, viewModel))
                    .ToArray()
                    ?? []
            };
        }

        private static WidgetAction CreateAction(
           WidgetActionDefinition definition,
           object viewModel)
            {
                return new WidgetAction
                {
                    Id = definition.Id,
                    Title = definition.Title,
                    IconKey = definition.IconKey,
                    Command = definition.CreateCommand(viewModel)
                };
            }

        private static WidgetAction? CreatePrimaryAction(
            WidgetActionDefinition? definition,
            object viewModel)
        {
            return definition is null
                ? null
                : CreateAction(definition, viewModel);
        }
    }
}
