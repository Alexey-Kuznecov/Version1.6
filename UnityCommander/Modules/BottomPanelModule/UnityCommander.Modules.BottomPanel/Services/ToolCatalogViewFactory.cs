
using System;
using System.Windows.Controls;
using UnityCommander.Modules.BottomPanel.ViewModels;
using UnityCommander.Modules.BottomPanel.Views;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.BottomPanel.Services
{
    public sealed class ToolCatalogViewFactory : IToolCatalogViewFactory
    {
        private readonly IToolRegistry _registry;

        public ToolCatalogViewFactory(IToolRegistry registry)
        {
            _registry = registry;
        }

        public Control Create(
            Action<IToolDescriptor> onSelected,
            Action onCanceled)
        {
            var viewModel = new ToolCatalogViewModel(
                _registry,
                onSelected,
                onCanceled);

            return new ToolCatalogView
            {
                DataContext = viewModel
            };
        }
    }
}
