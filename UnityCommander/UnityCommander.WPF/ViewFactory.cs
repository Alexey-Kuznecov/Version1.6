
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using UnityCommander.Services.Interfaces;
using IViewRegistry = UnityCommander.Core.Registrar.IViewRegistry;

namespace UnityCommander.WPF
{
    public class ViewFactory : IViewFactory
    {
        private readonly IViewRegistry _registry;
        private readonly IServiceScopeResolver _resolver;

        public ViewFactory(
            IViewRegistry registry,
            IServiceScopeResolver resolver)
        {
            _registry = registry;
            _resolver = resolver;
        }

        public FrameworkElement Create(object viewModel)
        {
            var viewType = _registry.GetView(viewModel.GetType())
                ?? throw new InvalidOperationException(
                    $"View is not registered for ViewModel '{viewModel.GetType().Name}'.");

            var view = (FrameworkElement)Activator.CreateInstance(viewType)!;
            view.DataContext = viewModel;

            return view;
        }

        public FrameworkElement Create<TViewModel>()
        {
            var viewModelType = typeof(TViewModel);

            var viewType = _registry.GetView(viewModelType)
                ?? throw new InvalidOperationException(
                    $"View is not registered for ViewModel '{viewModelType.Name}'.");

            var provider = _resolver.Resolve("");

            var viewModel = ActivatorUtilities.CreateInstance(
                provider,
                viewModelType);

            var view = (FrameworkElement)Activator.CreateInstance(viewType)!;
            view.DataContext = viewModel;

            return view;
        }
    }
}
