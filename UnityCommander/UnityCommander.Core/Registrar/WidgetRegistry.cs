
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Abstractions.Widget;

namespace UnityCommander.Core.Registrar
{
    public sealed class WidgetRegistry : IWidgetRegistry
    {
        private readonly Dictionary<string, WidgetDefinition> _widgets = new();

        public void Register<TViewModel, TView>(
            string id,
            string name,
            WidgetActionDefinition primaryAction,
            IReadOnlyList<WidgetActionDefinition> actions)
        {
            var definition = new WidgetDefinition(
                id,
                name,
                typeof(TViewModel),
                typeof(TView),
                primaryAction,
                actions);

            if (!_widgets.TryAdd(id, definition))
            {
                throw new InvalidOperationException(
                    $"Widget '{id}' is already registered.");
            }
        }

        public IReadOnlyList<WidgetDefinition> GetAll() =>
            _widgets.Values.ToList();

        public WidgetDefinition Get(string id) =>
            _widgets.TryGetValue(id, out var widget)
                ? widget
                : throw new KeyNotFoundException(
                    $"Widget '{id}' is not registered.");
    }
}
