
using Prism.Ioc;

using System;
using UnityCommander.Common.Helper;
using UnityCommander.Common.Plugins;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class ViewResolver : IViewResolver
    {
        private readonly IContainerProvider _container;
        private readonly IPluginProvider _pluginProvider;
        private readonly ILogger _logger;

        public ViewResolver(
            IContainerProvider container,
            IPluginProvider pluginProvider,
            LoggerCreator? logger)
        {
            _logger = logger?.For<ViewResolver>(
                scope: LogScope.Startup
            );

            _container = container;
            _pluginProvider = pluginProvider;
        }

        public object Resolve(Type type)
        {
            if (type == null)
            {
                _logger.Warning("Не удалось разрешить тип не указан");
                return null;
            }
            try
            {
                return _container.Resolve(type);
            }
            catch
            {
                var container = _pluginProvider.GetContainer("");
               
                var view = container.LoadedAssembly.CreateInstance(type.FullName); // (UserControl)Activator.CreateInstance(type);
                return view;
            }
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }
}