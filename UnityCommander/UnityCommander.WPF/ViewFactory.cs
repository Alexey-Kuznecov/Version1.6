
using System.Windows;
using UnityCommander.Services.Interfaces;
using IViewRegistry = UnityCommander.Core.Registrar.IViewRegistry;

namespace UnityCommander.WPF
{
    public class ViewFactory : IViewFactory
    {
        private readonly IViewRegistry _registry;
        private readonly IServiceScopeResolver _resolver;

        public ViewFactory(IViewRegistry registry, IServiceScopeResolver resolver)
        {
            _registry = registry;
            _resolver = resolver;
        }

        public FrameworkElement Create(object viewModel)
        {
            var provider = _resolver.Resolve("");

            var viewType = _registry.GetView(viewModel.GetType())
                ?? throw new InvalidOperationException();

            var view = (FrameworkElement)Activator.CreateInstance(viewType)!;
            view.DataContext = viewModel;

            return view;
        }
    }
}
