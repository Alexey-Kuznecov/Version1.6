
using System;
using System.Collections.Concurrent;

namespace UnityCommander.Core.Registrar
{
    public class ViewRegistry : IViewRegistry
    {
        private readonly ConcurrentDictionary<Type, Type> _views = new();

        public void Register<TViewModel, TView>()
        {
            if (!_views.TryAdd(typeof(TViewModel), typeof(TView)))
            {
                throw new InvalidOperationException(
                    $"View for '{typeof(TViewModel).FullName}' is already registered.");
            }
        }

        public Type? GetView(Type viewModelType)
        {
            return _views.TryGetValue(viewModelType, out var view)
                ? view
                : null;
        }
    }
}
