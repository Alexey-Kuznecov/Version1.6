
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Windows.Controls;
using UnityCommander.Abstractions.Plugins;

namespace UnityCommander.Abstractions.Plugin
{
    public class CompositionEngine
    {
        private readonly ICompositionRegistry _registry;

        public CompositionEngine(ICompositionRegistry registry)
        {
            _registry = registry;
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

                InjectIntoWindow(window, part.Region, view);
            }

            return window;
        }

        private void InjectIntoWindow(object window, string region, object view)
        {
            if (string.IsNullOrWhiteSpace(region))
                return;

            var field = window.GetType().GetField(
                region,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

            if (field == null)
                throw new InvalidOperationException(
                    $"Region '{region}' not found in window '{window.GetType().Name}'.");

            if (field.GetValue(window) is ContentControl contentControl)
            {
                contentControl.Content = view;
                return;
            }

            throw new InvalidOperationException(
                $"Region '{region}' is not a ContentControl.");
        }
    }
}
