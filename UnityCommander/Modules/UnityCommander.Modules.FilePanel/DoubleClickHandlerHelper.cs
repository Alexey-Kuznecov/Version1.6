#undef DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using UnityCommander.Logging.Contracts;
using Xceed.Wpf.AvalonDock;

namespace UnityCommander.Modules.FilePanel
{
    // Хелпер для обработки двойного клика
    public class DoubleClickHandlerHelper
    {
        private readonly ILogger _logger;

        private DateTime _lastDoubleClickHandled = DateTime.MinValue;

        public DoubleClickHandlerHelper(ILogger logger)
        {
            _logger = logger;
        }

        public bool HandleDoubleClick(DockingManager dockObj, DependencyObject hitVisual, DateTime lastNavigationTime)
        {
            try
            {
                if ((DateTime.UtcNow - lastNavigationTime).TotalMilliseconds < 300)
                {
#if DEBUG
                    _logger.Info("DoubleClick: ignored due to recent navigation");
#endif
                    return false;
                }

                if (dockObj == null || hitVisual == null)
                    return false;

                var fullChain = BuildVisualChain(hitVisual);
                LogChain(fullChain);

                if (!ContainsPane(fullChain)) return false;
                if (HitCloseBlacklist(fullChain)) return false;
                if (HitContentPresenterTooClose(fullChain, hitVisual)) return false;
                if (HitBroaderBlacklist(fullChain)) return false;
                if (IsDoubleClickCooldown()) return false;
            }
            catch (Exception ex)
            {
#if DEBUG
                _logger.Info("DoubleClickHandlerHelper error: " + ex);
#endif
            }

            return true;
        }

        private List<string> BuildVisualChain(DependencyObject start)
        {
            var chain = new List<string>();
            var node = start;
            while (node != null)
            {
                chain.Add(node.GetType().Name);
                node = GetParentSafely(node);
            }
            return chain;
        }

        private void LogChain(List<string> chain)
        {
#if DEBUG
            _logger.Info("DoubleClick Hit chain: " + string.Join(" -> ", chain));
#endif
        }

        private bool ContainsPane(List<string> chain)
        {
            if (!chain.Any(n => n == "LayoutDocumentPaneControl"))
            {
#if DEBUG
                _logger.Info("DoubleClick: no LayoutDocumentPaneControl -> reject");
#endif
                return false;
            }
            return true;
        }

        private bool HitCloseBlacklist(List<string> chain)
        {
            int checkDepth = 6;
            var firstNodes = chain.Take(checkDepth).ToArray();
            var closeBlacklist = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "ListViewItem","ListView","ListBoxItem","GridViewRowPresenter",
            "TextBlock","TreeViewItem","ScrollViewer","GridView","Path","Button","Image","ContentControl"
        };

            var hits = firstNodes.Where(n => closeBlacklist.Contains(n)).ToArray();
            if (hits.Any())
            {
#if DEBUG
                _logger.Info("DoubleClick: close-blacklist hit -> reject (" + string.Join(", ", hits) + ")");
#endif
                return true;
            }

            return false;
        }

        private bool HitContentPresenterTooClose(List<string> chain, DependencyObject hitVisual)
        {
            if (hitVisual.GetType().Name != "LayoutDocumentControl")
            {
                int idxContentPresenter = chain.FindIndex(n => n == "ContentPresenter");
                if (idxContentPresenter >= 0 && idxContentPresenter <= 2)
                {
#if DEBUG
                    _logger.Info("DoubleClick: ContentPresenter close to hit -> reject");
#endif
                    return true;
                }
            }
            return false;
        }

        private bool HitBroaderBlacklist(List<string> chain)
        {
            var broaderBlacklist = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "ToolBar","ToolBarTray","MenuItem","ContextMenu","Thumb","Popup"
        };

            if (chain.Any(n => broaderBlacklist.Contains(n)))
            {
#if DEBUG
                _logger.Info("DoubleClick: broader blacklist -> reject");
#endif
                return true;
            }
            return false;
        }

        private bool IsDoubleClickCooldown()
        {
            if ((DateTime.UtcNow - _lastDoubleClickHandled).TotalMilliseconds < 200)
            {
#if DEBUG
                _logger.Info("DoubleClick: suppressed due to double handling cooldown");
#endif
                return true;
            }
            _lastDoubleClickHandled = DateTime.UtcNow;
            return false;
        }

        private DependencyObject GetParentSafely(DependencyObject node)
        {
            if (node is Visual || node is Visual3D)
                return VisualTreeHelper.GetParent(node);
            return null;
        }
    }
}
