

using Microsoft.Extensions.DependencyInjection;
using System;
using UnityCommander.Abstractions.Plugins;
using UnityCommander.Core.Plugin;

namespace UnityCommander.Abstractions.Plugin
{
    public class CompositionEngine
    {
        private readonly ICompositionRegistry _registry;
        private readonly IRegionInjector _injector;

        public CompositionEngine(ICompositionRegistry registry, IRegionInjector injector)
        {
            _registry = registry;
            _injector = injector;
        }

        #nullable enable
        public object Create(Type windowType, PluginCompositionContext context, object? parameter = null)
        {
            var sp = context.Services;

            var def = _registry.Get(windowType);

            var window = ActivatorUtilities.CreateInstance(sp, windowType);

            foreach (var part in def.Parts)
            {
                var vm = sp.GetRequiredService(part.ViewModel);
                var view = ActivatorUtilities.CreateInstance(sp, part.View);

                if (vm is IInitializable init)
                {
                    init.Initialize(parameter);
                }

                view.GetType()
                    .GetProperty("DataContext")
                    ?.SetValue(view, vm);

                _injector.Inject(window, part.Region, view);
            }

            return window;
        }
    }
}
