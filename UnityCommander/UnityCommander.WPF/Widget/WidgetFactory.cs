
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
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

        public FrameworkElement Create(string widgetId)
        {
            var definition = _registry.Get(widgetId);
            var provider = _scopeResolver.Resolve("");

            var viewModel = ActivatorUtilities.CreateInstance(
                provider,
                definition.ViewModelType);

            var view = (FrameworkElement)
                Activator.CreateInstance(definition.ViewType)!;

            view.DataContext = viewModel;

            return view;
        }
    }
}
