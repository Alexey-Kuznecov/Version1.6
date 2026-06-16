
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public sealed class WindowManager : IWindowManager
    {
        private readonly IDialogRegistry _registry;
        private readonly IServiceScopeResolver _resolver;

        public WindowManager(
            IDialogRegistry registry,
            IServiceScopeResolver resolver)
        {
            _resolver = resolver;
            _registry = registry;
        }

        public bool ShowDialog(string id)
        {
            if (!_registry.TryGet(id, out var registration))
                return false;

            var (window, viewModel) = CreateWindow(registration);

            window.Show();

            return true;
        }

        public bool? ShowModalDialog(string id)
        {
            if (!_registry.TryGet(id, out var registration))
                return null;

            var (window, viewModel) = CreateWindow(registration);

            return window.ShowDialog();
        }

        public TDialogResult? ShowModalDialog<TDialogResult>(
          string id,
          object? parameter = null)
          where TDialogResult : IDialogResult
        {
            if (!_registry.TryGet(id, out var registration))
                return default;

            var (window, viewModel) =
                CreateWindow(registration);

            if (viewModel is IDialogAware<TDialogResult> dialogAware)
            {
                dialogAware.RequestClose = window.Close;

                dialogAware.OnDialogOpened(parameter);

                window.Closed += (_, _) =>
                {
                    dialogAware.OnDialogClosed();

                    dialogAware.RequestClose = null;
                };

                window.Closing += (_, e) =>
                {
                    if (!dialogAware.CanCloseDialog())
                        e.Cancel = true;
                };
            }

            window.ShowDialog();

            return (viewModel as IDialogAware<TDialogResult>).Result;
        }

        private (Window Window, object ViewModel) CreateWindow(IDialogDefinition registration)
        {
            var provider = _resolver.Resolve(registration.OwnerId);

            var options = registration.Options ?? new DialogOptions();

            var view =
                (FrameworkElement)ActivatorUtilities.CreateInstance(
                    provider,
                    registration.ViewType);

            var viewModel =
                ActivatorUtilities.CreateInstance(
                     provider,
                    registration.ViewModelType);

            view.DataContext = viewModel;

            var window = new Window
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

            return (window, viewModel);
        }
    }
}
