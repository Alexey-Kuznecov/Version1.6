
using Microsoft.Extensions.DependencyInjection;
using PluginSystem.Abstractions;
using PluginSystem.Abstractions.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Abstractions;
using UnityCommander.Abstractions.Columns;
using UnityCommander.Abstractions.Command;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Abstractions.Icons;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Abstractions.Plugin;
using UnityCommander.Abstractions.Plugins;
using UnityCommander.Abstractions.Ribbon;
using UnityCommander.Abstractions.Sidebar;

namespace UnityCommander.Core.Plugin
{
    public class PluginRegistrar : IPluginRegistrar
    {
        private readonly string _pluginId;

        private readonly List<ICommandDefinition> _commands = new();

        private readonly List<ServiceDescriptor> _services = new();

        private readonly List<IDisposable> _disposables = new();

        private readonly List<ISidebarSection> _sidebarSection = new();

        private readonly List<IDialogDefinition> _dialogDefinitions = new();

        private readonly List<IColumnProvider> _columnProviders = new();

        private readonly List<IIconSource> _iconSources = new();

        private readonly List<CompositionDefinition> _compositions = new();

        private readonly List<ServiceOverrideEntry> _serviceOverrideEntry = new();

        private RibbonContribution _ribbonContribution;

        public PluginRegistrar(string pluginId)
            => _pluginId = pluginId;

        public Dictionary<Type, Type> Views { get; } = new();

        //public List<UserControl> ViewInstance { get; } = new();

        public IReadOnlyList<ServiceDescriptor> Services => _services;

        public void RegisterDisposable(IDisposable disposable) => _disposables.Add(disposable);

        public void RegisterSingleton<TService, TImpl>()
            where TService : class
            where TImpl : class, TService
        {
            _services.Add(
                ServiceDescriptor.Singleton<TService, TImpl>());
        }

        public void RegisterTransient<TService, TImpl>()
            where TService : class
            where TImpl : class, TService
        {
            _services.Add(
                ServiceDescriptor.Transient<TService, TImpl>());
        }

        public void RegisterSingleton<TService>(
           Func<IServiceProvider, TService> factory)
           where TService : class
        {
            _services.Add(
               ServiceDescriptor.Singleton(factory));
        }

        public void RegisterInstance<TService>(TService instance)
            where TService : class
        {
            _services.Add(
                ServiceDescriptor.Singleton(typeof(TService), instance));
        }

        public void RegisterTransient<T>() where T : class
        {
            _services.Add(ServiceDescriptor.Transient(typeof(T), typeof(T)));
        }

        public void RegisterSingleton<T>() where T : class
        {
            _services.Add(
                ServiceDescriptor.Singleton(
                    typeof(T), typeof(T)));
        }

        public void ConfigureRibbon(
            Action<RibbonBuilder> configure)
        {
            var ribbon = new RibbonDefinition();

            var builder = new RibbonBuilder(ribbon);

            configure(builder);

            _ribbonContribution =
                new RibbonContribution(
                    _pluginId,
                    ribbon);
        }

        public void RegisterCommand(ICommandDefinition command)
        {
            StampPluginOwnership(command, _pluginId);
            _commands.Add(command);
        }

        public void RegisterSidebarItem(ISidebarSection section)
            => _sidebarSection.Add(section);

        public void RegisterDialog(IDialogDefinition dialog)
            => _dialogDefinitions.Add(dialog);

        public void RegisterIconSource(IIconSource source)
        {
            StampPluginOwnership(source, _pluginId);
            _iconSources.Add(source);
        }

        public void RegisterEventHandler(Delegate handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
        }

        public void UnregisterEventHandler(Delegate handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
        }

        //public void RegisterView<TView, TViewModel>()
        //    where TView : UserControl
        //{
        //    var view = (UserControl)Activator.CreateInstance(typeof(TView))!;

        //    view.DataContext = Activator.CreateInstance(typeof(TViewModel))!;

        //    ViewInstance.Add(view);

        //    Views[typeof(TView)] = typeof(TViewModel);
        //}

        public void RegisterColumn<TProvider, TImplement>()
            where TProvider : IColumnProvider
            where TImplement : class, TProvider
        {
            _columnProviders.Add(Activator.CreateInstance<TImplement>());
        }

        public void RegisterOverride<TService, TImplementation>()
            where TImplementation : class, TService
        {
            _serviceOverrideEntry.Add(new ServiceOverrideEntry()
            {
                OwnerId = _pluginId,
                ImplementationType = typeof(TImplementation),
                ServiceType = typeof(TService)
            });
        }

        public void ConfigureComposition<TWindow>(
            Action<ICompositionBuilder> configure)
        {
            var def = new CompositionDefinition(typeof(TWindow));

            StampPluginOwnership(def, _pluginId);

            var builder = new CompositionBuilder(def);

            configure(builder);

            _compositions.Add(def);
        }

        public void Apply(IServiceProvider serviceProvider)
        {
            var runtime = serviceProvider.GetRequiredService<IRuntimeServices>();

            foreach (var section in _sidebarSection)
            {
                StampPluginOwnership(section, _pluginId);
                runtime.Sidebar.Register(section);
            }

            foreach (var dialog in _dialogDefinitions)
            {
                StampPluginOwnership(dialog, _pluginId);
                runtime.Dialog.Register(dialog);
            }

            foreach (var provider in _columnProviders)
            {
                runtime.Columns.RegisterPluginProvider(_pluginId, provider);
            }

            foreach (var entry in _serviceOverrideEntry)
            {
                runtime.Overrides.Register(
                    _pluginId,
                    entry.ServiceType,
                    entry.ImplementationType);
            }

            foreach (var composition in _compositions)
            {
                runtime.Composition.Register(composition);
            }

            foreach (var command in _commands)
            {
                runtime.Commands.Register(command);
            }

            if (_ribbonContribution != null)
                runtime.Ribbon.Register(_ribbonContribution);

            RegisterInfrastructureServices();
        }

        private void StampPluginOwnership(object item, string pluginId)
        {
            if (item is IOwned owned)
            {
                owned.OwnerId = pluginId;
            }
        }

        private void RegisterInfrastructureServices()
        {
            _services.Add(ServiceDescriptor.Singleton(typeof(IMessageBus), typeof(MessageBus)));
        }

        public T GetRequired<T>()
        {
            var descriptor = Services.FirstOrDefault(x => x.ServiceType == typeof(T));
            return (T)Activator.CreateInstance(descriptor.ImplementationType);
        }

        private readonly List<Func<IServiceProvider, object>> _deferred = new ();

        public void RegisterIconSource<T>()
            where T : IIconSource
        {
            _deferred.Add(provider =>
            {
                var source = provider.GetRequiredService<T>();
                _iconSources.Add(source);

                return source;
            });
        }

        public void Initialize(
             IServiceProvider pluginProvider,
             IServiceProvider rootProvider)
        {
            foreach (var deferred in _deferred)
                deferred(pluginProvider);

            var runtime = rootProvider.GetRequiredService<IRuntimeServices>();

            foreach (var source in _iconSources)
                runtime.Icons.Register(source);

            _iconSources.Clear();
            _deferred.Clear();
        }
    }
}
