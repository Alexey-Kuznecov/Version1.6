using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Core.Commands
{
    public class NavigationManager : INavigationService
    {
        private readonly Stack<string?> _back = new();
        private readonly Stack<string?> _forward = new();

        public string? Current { get; private set; } // null => "Мой компьютер"

        public event Action<string?>? CurrentChanged;

        private readonly Func<string?, bool> _pathValidator;

        /// <summary>
        /// Передайте валидатор пути. Если null — все пути считаются валидными.
        /// </summary>
        public NavigationManager(Func<string?, bool>? pathValidator = null)
        {
            _pathValidator = pathValidator ?? (_ => true);
            Current = null; // по умолчанию "Мой компьютер"
        }

        public bool IsValidPath(string? path) => _pathValidator(path);

        public void NavigateTo(string path)
        {
            // Normalize: null => root
            if (!IsValidPath(path))
                throw new InvalidOperationException($"Path is invalid: {path ?? "<root>"}");

            if (Current != null)
                _back.Push(Current);

            Current = path;
            _forward.Clear();
            CurrentChanged?.Invoke(Current);
        }

        public bool TryNavigateTo(string? path)
        {
            if (!IsValidPath(path)) return false;
            NavigateTo(path);
            return true;
        }

        public bool CanGoBack => _back.Count > 0;
        public bool CanGoForward => _forward.Count > 0;

        public void GoBack()
        {
            if (!CanGoBack) return;

            _forward.Push(Current);
            Current = _back.Pop();
            CurrentChanged?.Invoke(Current);
        }

        public void GoForward()
        {
            if (!CanGoForward) return;

            _back.Push(Current);
            Current = _forward.Pop();
            CurrentChanged?.Invoke(Current);
        }

        /// <summary>Очищает историю (не меняет Current).</summary>
        public void ClearHistory()
        {
            _back.Clear();
            _forward.Clear();
        }

        // Реализация INavigationService:
        bool INavigationService.TryNavigateTo(string? path) => TryNavigateTo(path);
        bool INavigationService.IsValidPath(string? path) => IsValidPath(path);
        event Action<string?>? INavigationService.CurrentChanged
        {
            add => CurrentChanged += value;
            remove => CurrentChanged -= value;
        }
    }
}
