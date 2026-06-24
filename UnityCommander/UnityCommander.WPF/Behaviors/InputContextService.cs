
using System.Windows;
using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.WPF.Behaviors
{
    public sealed class InputContextService : IInputContextService
    {
        private readonly Dictionary<Window, ShortcutScope> _scopes = new();
        private readonly Stack<Window> _stack = new();

        public Window? ActiveWindow =>
            _stack.Count > 0 ? _stack.Peek() : null;

        public ShortcutScope CurrentScope =>
            ActiveWindow != null
                ? _scopes[ActiveWindow]
                : ShortcutScope.Global;

        public void Attach(Window window, ShortcutScope scope)
        {
            _scopes[window] = scope;
        }

        public void Detach(Window window)
        {
            _scopes.Remove(window);

            // чистим стек
            var temp = _stack.Where(x => x != window).ToArray();
            _stack.Clear();

            foreach (var w in temp.Reverse())
                _stack.Push(w);
        }

        public void SetActive(Window window)
        {
            if (_stack.Count > 0 && _stack.Peek() == window)
                return;

            _stack.Push(window);
        }
    }
}
