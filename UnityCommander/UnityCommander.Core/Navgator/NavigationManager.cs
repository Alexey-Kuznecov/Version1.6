using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Core.Navgator
{
    public class NavigationManager : INavigationService
    {
        private readonly Stack<string> _back = new();
        private readonly Stack<string> _forward = new();

        public string? Current { get; private set; }
        public event Action<string?>? CurrentChanged;
        private readonly Func<string?, bool> _pathValidator;

        public NavigationManager(Func<string?, bool>? pathValidator = null)
        {
            if (pathValidator != null)
                _pathValidator = pathValidator;
            else
                _pathValidator = path => Directory.Exists(path) || VirtualPaths.MyComputer == path;

            Current = null;
        }

        public bool IsValidPath(string? path) => _pathValidator(path);

        // перегруженный TryNavigateTo с флагом
        public bool TryNavigateTo(string? path, bool forceRecord = false)
        {
            if (!IsValidPath(path)) return false;
            NavigateTo(path, forceRecord);
            return true;
        }

        // Основной метод: выбирай recordOnSame (false по умолчанию)
        public void NavigateTo(string? path, bool recordOnSame = false)
        {
            if (!IsValidPath(path))
                throw new InvalidOperationException($"Path is invalid: {path ?? "<root>"}");

            // Если не нужно записывать одинаковый путь и он совпадает — просто возвращаемся
            if (!recordOnSame && Equals(Current, path))
                return;

            // Пушим текущий в back (даже если он равен path, если recordOnSame==true)
            if (Current != null)
                _back.Push(Current);

            Current = path;
            _forward.Clear();
            CurrentChanged?.Invoke(Current);
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

        public void ClearHistory()
        {
            _back.Clear();
            _forward.Clear();
        }

        // Реализация INavigationService:
        bool INavigationService.TryNavigateTo(string path) => TryNavigateTo(path);
        bool INavigationService.IsValidPath(string path) => IsValidPath(path);
        event Action<string> INavigationService.CurrentChanged
        {
            add => CurrentChanged += value;
            remove => CurrentChanged -= value;
        }
    }
}
