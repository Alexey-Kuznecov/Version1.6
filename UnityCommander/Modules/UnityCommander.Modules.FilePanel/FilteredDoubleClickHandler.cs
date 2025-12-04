using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace UnityCommander.Modules.FilePanel
{
    public class FilteredDoubleClickHandler
    {
        private readonly FrameworkElement _target;
        private readonly Action _onDoubleClick;

        private readonly HashSet<string> _closeBlacklist;
        private readonly HashSet<string> _broadBlacklist;
        private readonly HashSet<string> _requiredTypes;

        private readonly int _checkDepth;
        private readonly int _debounceMs;
        private DateTime _lastHandled = DateTime.MinValue;

        public FilteredDoubleClickHandler(
            FrameworkElement target,
            Action onDoubleClick,
            IEnumerable<string> closeBlacklist = null,
            IEnumerable<string> broadBlacklist = null,
            IEnumerable<string> requiredTypes = null,
            int checkDepth = 6,
            int debounceMs = 200
        )
        {
            _target = target;
            _onDoubleClick = onDoubleClick;

            _closeBlacklist = new HashSet<string>(closeBlacklist ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);
            _broadBlacklist = new HashSet<string>(broadBlacklist ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);
            _requiredTypes = new HashSet<string>(requiredTypes ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);

            _checkDepth = checkDepth;
            _debounceMs = debounceMs;

            //_target.MouseDoubleClick += OnMouseDoubleClick;
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if ((DateTime.UtcNow - _lastHandled).TotalMilliseconds < _debounceMs)
                    return;

                var pos = e.GetPosition(_target);
                var hit = VisualTreeHelper.HitTest(_target, pos);
                if (hit == null) return;

                var start = hit.VisualHit as DependencyObject;
                if (start == null) return;

                // Собираем цепочку родителей
                var chain = new List<string>();
                var node = start;
                while (node != null)
                {
                    chain.Add(node.GetType().Name);
                    node = GetParentSafely(node);
                }

                // 1) Проверка required типов (например LayoutDocumentPaneControl)
                if (_requiredTypes.Count > 0)
                {
                    bool ok = chain.Any(n => _requiredTypes.Contains(n));
                    if (!ok) return;
                }

                // 2) Проверка "близкого" blacklist
                var firstNodes = chain.Take(_checkDepth);
                if (firstNodes.Any(n => _closeBlacklist.Contains(n)))
                    return;

                // 3) Проверка широкого blacklist
                if (chain.Any(n => _broadBlacklist.Contains(n)))
                    return;

                // Всё прошло — вызываем callback
                _lastHandled = DateTime.UtcNow;
                _onDoubleClick?.Invoke();
            }
            catch
            {
                // ignore
            }
        }

        private DependencyObject GetParentSafely(DependencyObject node)
        {
            if (node is Visual or Visual3D)
                return VisualTreeHelper.GetParent(node);

            return LogicalTreeHelper.GetParent(node) as DependencyObject;
        }
    }

}
