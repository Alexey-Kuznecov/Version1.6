#undef DEBUG

using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using UnityCommander.Common.Module;
using UnityCommander.Modules.FilePanel.Views;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;
using Xceed.Wpf.AvalonDock;

namespace UnityCommander.Modules.FilePanel
{
    // Хелпер для обработки двойного клика
    public class DoubleClickHandlerHelper
    {
        private readonly IAppLogger _appLogger;
        private readonly CommandService _commandExecute;
        private readonly IDockingService _dockingService;
        private readonly IRegionManager _regionManager;

        private DateTime _lastDoubleClickHandled = DateTime.MinValue;

        public DoubleClickHandlerHelper(
            IAppLogger appLogger,
            CommandService commandExecute,
            IDockingService dockingService,
            IRegionManager regionManager)
        {
            _appLogger = appLogger;
            _commandExecute = commandExecute;
            _dockingService = dockingService;
            _regionManager = regionManager;
        }

        public void HandleDoubleClick(DockingManager dockObj, DependencyObject hitVisual, DateTime lastNavigationTime)
        {
            try
            {
                if ((DateTime.UtcNow - lastNavigationTime).TotalMilliseconds < 300)
                {
#if DEBUG
                    _appLogger.Info("DoubleClick: ignored due to recent navigation");
#endif
                    return;
                }

                if (dockObj == null || hitVisual == null)
                    return;

                var fullChain = BuildVisualChain(hitVisual);
                LogChain(fullChain);

                if (!ContainsPane(fullChain)) return;
                if (HitCloseBlacklist(fullChain)) return;
                if (HitContentPresenterTooClose(fullChain, hitVisual)) return;
                if (HitBroaderBlacklist(fullChain)) return;
                if (IsDoubleClickCooldown()) return;

                ProcessNavigation();
            }
            catch (Exception ex)
            {
#if DEBUG
                _appLogger.Info("DoubleClickHandlerHelper error: " + ex);
#endif
            }
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
            _appLogger.Info("DoubleClick Hit chain: " + string.Join(" -> ", chain));
#endif
        }

        private bool ContainsPane(List<string> chain)
        {
            if (!chain.Any(n => n == "LayoutDocumentPaneControl"))
            {
#if DEBUG
                _appLogger.Info("DoubleClick: no LayoutDocumentPaneControl -> reject");
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
                _appLogger.Info("DoubleClick: close-blacklist hit -> reject (" + string.Join(", ", hits) + ")");
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
                    _appLogger.Info("DoubleClick: ContentPresenter close to hit -> reject");
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
                _appLogger.Info("DoubleClick: broader blacklist -> reject");
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
                _appLogger.Info("DoubleClick: suppressed due to double handling cooldown");
#endif
                return true;
            }
            _lastDoubleClickHandled = DateTime.UtcNow;
            return false;
        }

        private void ProcessNavigation()
        {
            var context = _commandExecute.Execute("getcurpath");
            var basePath = context.Result as string;

            // Сохранения реального пути вкладки
            var contentId = Guid.NewGuid(); // уникальный ID вкладки

            // Формируем короткий путь для отображения в вкладке
            var tabHeader = PathTitleHelper.GetTabTitle(basePath);

            //var token = Guid.NewGuid();
            var regionName = $"Tab_{contentId}";

            // Передаём короткий путь как заголовок вкладки
            _dockingService.AddActiveDocumentTab(tabHeader, basePath, regionName);

            _regionManager.RequestNavigate(regionName, nameof(SplitPanelView), result =>
            {
                if (result.Result == true)
                {
                    var view = result.Context.NavigationService.Region.ActiveViews
                                   .FirstOrDefault() as SplitPanelView;
                    var viewModel = view?.DataContext as ITabPanelContent;
                    viewModel?.InitializedViewModel(ref contentId, basePath);
                }
            });
        }

        private DependencyObject GetParentSafely(DependencyObject node)
        {
            if (node is Visual || node is Visual3D)
                return VisualTreeHelper.GetParent(node);
            return null;
        }
    }
}
