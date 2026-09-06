
using System.Windows;

namespace UnityCommander.UI.Visual
{
    public sealed class VisualElementRegistry : IVisualElementRegistry
    {
        private readonly Dictionary<object, WeakReference<FrameworkElement>> _elements = [];

        public void Register(object model, FrameworkElement element)
        {
            _elements[model] = new WeakReference<FrameworkElement>(element);
        }

        public void Unregister(object model, FrameworkElement element)
        {
            if (!_elements.TryGetValue(model, out var reference))
                return;

            if (!reference.TryGetTarget(out var target) || ReferenceEquals(target, element))
                _elements.Remove(model);
        }

        public FrameworkElement? GetElement(object model)
        {
            if (!_elements.TryGetValue(model, out var reference))
                return null;

            if (reference.TryGetTarget(out var element))
                return element;

            _elements.Remove(model);
            return null;
        }
    }
}
