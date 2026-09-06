
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Abstractions.Plugin;
using UnityCommander.Abstractions.Plugins;
using UnityCommander.Core.Plugin;
using UnityCommander.Services.Interfaces;
using UnityCommander.WPF.Dialog;
using IDialogResult = UnityCommander.Abstractions.Dialog.IDialogResult;

namespace UnityCommander.Services
{
    public sealed class WindowManager : IWindowManager
    {
        private readonly IDialogRegistry _registry;
        private readonly CompositionEngine _engine;
        private readonly IServiceScopeResolver _resolver;
        private readonly ICompositionRegistry _composition;
        private readonly WindowInputManager _windowInput;
        private PluginHost _host;

        public WindowManager(
            IDialogRegistry registry,
            IServiceScopeResolver resolver, 
            ICompositionRegistry composition,
            WindowInputManager windowInput,
            CompositionEngine compositionEngine,
            PluginHost host)
        {
            _windowInput = windowInput;
            _resolver = resolver;
            _registry = registry;
            _engine = compositionEngine;
            _composition = composition;
            _host = host;
        }

        public bool ShowDialog(string id)
        {
            if (!_registry.TryGet(id, out var registration))
                return false;

            var (window, viewModel) = CreateWindow(registration);

            window.Show();

            return true;
        }

        #nullable enable
        public bool? ShowDialog<TDialog>(object? paramerter = null)
        {
            var type = typeof(TDialog);

            // Сначала ищем композит
            if (_composition.TryGet(type, out var composition))
            {
                var plugin = _host.Get(composition.OwnerId);

                var obj = _engine.Create(typeof(TDialog),
                    new PluginCompositionContext()
                    {
                        PluginId = plugin.PluginId,
                        Services = plugin.Services
                    }, paramerter);

                if (obj is UserControl)
                {
                    var window = new Window
                    {
                        Content = obj,

                        Owner = Application.Current.MainWindow,

                        WindowStartupLocation =
                        WindowStartupLocation.CenterOwner,
                    };

                    window.Show();
                }

                if (obj is Window customWindow)
                {
                    customWindow.Show();

                    //_windowInput.Attach(customWindow, ShortcutScope.Global);

                    customWindow.Owner = Application.Current.MainWindow;
                }
            }

            // Потом обычный диалог
            if (_registry.TryGet<TDialog>(out var dialog))
            {
                var (window, viewModel) = CreateWindow(dialog);

                _windowInput.Attach(window, ShortcutScope.Dialog);

                return window.ShowDialog();
            }

            return false;
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
                Title = options.Title,
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
