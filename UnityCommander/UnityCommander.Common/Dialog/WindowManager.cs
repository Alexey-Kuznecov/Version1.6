
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using UnityCommander.Common.Plugins;

namespace UnityCommander.Common.Dialog
{
    public sealed class WindowManager : IWindowManager
    {
        private readonly IDialogRegistry _registry;
        private readonly IPluginProvider _provider;

        public WindowManager(
            IDialogRegistry registry,
            IPluginProvider provider)
        {
            _provider = provider;
            _registry = registry;
        }

        public bool ShowDialog(string id)
        {
            if (!_registry.TryGet(id, out var registration))
                return false;

            var window = CreateWindow(registration);

            window.Show();

            return true;
        }

        public bool? ShowModalDialog(string id)
        {
            if (!_registry.TryGet(id, out var registration))
                return null;

            var window = CreateWindow(registration);

            return window.ShowDialog();
        }

        private Window CreateWindow(IDialogDefinition registration)
        {
            var container = _provider.GetContainer(registration.OwnerId);

            var options = registration.Options ?? new DialogOptions();

            var view =
                (FrameworkElement)ActivatorUtilities.CreateInstance(
                    container.Services,
                    registration.ViewType);

            var viewModel =
                ActivatorUtilities.CreateInstance(
                     container.Services,
                    registration.ViewModelType);

            view.DataContext = viewModel;

            return new Window
            {
                Content = view,
                Width = options.Width,
                Height = options.Height,

                Owner = Application.Current.MainWindow,

                WindowStartupLocation =
                    WindowStartupLocation.CenterOwner,

                ResizeMode =
                    options.IsResizable
                        ? ResizeMode.CanResize
                        : ResizeMode.NoResize
            };
        }
    }
}
