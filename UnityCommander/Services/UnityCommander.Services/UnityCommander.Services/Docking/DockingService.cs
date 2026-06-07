using Prism.Mvvm;
using Prism.Navigation.Regions;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UnityCommander.Common.Module;
using UnityCommander.Services.Interfaces;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace UnityCommander.Services.Docking
{
    public class DockingService : IDockingService
    {
        private DockingManager _dockingManager;
        private DockingSyncContext _dockingSyncContext;

        public event EventHandler ActiveContentChanged;

        public DockingService(DockingManager dockingManager, DockingSyncContext dockingSyncContext)
        {
            _dockingManager = dockingManager;
            _dockingSyncContext = dockingSyncContext;
        }

        public void SetDockingManager(DockingManager dockingManager) =>_dockingManager = dockingManager;
        
        public DockingManager GetDockingManager() => _dockingManager;

        public void ShowDocument(UserControl view, string title)
        {
            var doc = new LayoutDocument
            {
                Title = title,
                Content = view
            };

            var documentsPane = _dockingManager.Layout.Descendents()
                .OfType<LayoutDocumentPane>().FirstOrDefault();

            if (documentsPane == null)
            {
                var newPane = new LayoutDocumentPane();
                _dockingManager.Layout.RootPanel.Children.Add(newPane);
                documentsPane = newPane;
            }

            documentsPane.Children.Add(doc);
            doc.IsSelected = true;
        }

        public void AddDocumentTab(string title, string realPath, string regionName)
        {
            var contentControl = new ContentControl();
            RegionManager.SetRegionName(contentControl, regionName);
            ViewModelLocator.SetAutoWireViewModel(contentControl, true);
            
            var document = new LayoutDocument
            {
                Title = title,
                Content = contentControl,
                ContentId = realPath // важно для восстановления
            };

            _dockingSyncContext.GetOrCreateTabId(document); // Вот это смотри
            _dockingManager.Layout.Descendents()
                .OfType<LayoutDocumentPane>()
                .First()
                .Children.Add(document);
        }

        public void AddActiveDocumentTab(string tabId, string title, string regionName)
        {
            var contentControl = new ContentControl();
            RegionManager.SetRegionName(contentControl, regionName);
            ViewModelLocator.SetAutoWireViewModel(contentControl, true);

            var document = new LayoutDocument
            {
                Title = title,
                Content = contentControl,
                ContentId = tabId // важно для восстановления
            };

            // 🔥 Подписка после загрузки (когда VM уже создана)
            contentControl.Loaded += (s, e) =>
            {
                if (GetActiveDirectoryPanel() is IDirectoryPanel panel)
                {
                    panel.TabTitleChanged += formatPath =>
                    {
                        document.Title = formatPath; // обновляем вкладку
                    };
                }
            };

            var activePane = GetActiveDocumentPane();
            if (activePane != null)
            {
                activePane.Children.Add(document);
                document.IsActive = true;
            }
            else
            {
                // fallback
                var firstPane = _dockingManager.Layout
                    .Descendents()
                    .OfType<LayoutDocumentPane>()
                    .FirstOrDefault();

                firstPane?.Children.Add(document);
                document.IsActive = true;
            }
        }

        private LayoutDocumentPane GetActiveDocumentPane()
        {
            var activeContent = _dockingManager.Layout.ActiveContent as LayoutDocument;
            if (activeContent == null)
                return null;

            return activeContent.Parent as LayoutDocumentPane;
        }

        public void ShowAnchorable(UserControl view, string title, bool state, AnchorableShowStrategy strategy = AnchorableShowStrategy.Left)
        {
            var anchorable = new LayoutAnchorable
            {
                Title = title,
                Content = view,
                CanClose = false,
                CanHide = true
            };

            LayoutAnchorGroup anchorablePane = null;

            switch (strategy)
            {
                case AnchorableShowStrategy.Left:
                    anchorablePane = _dockingManager.Layout.LeftSide.Children.FirstOrDefault();
                    break;
                case AnchorableShowStrategy.Right:
                    anchorablePane = _dockingManager.Layout.RightSide.Children.FirstOrDefault();
                    break;
                case AnchorableShowStrategy.Bottom:
                    anchorablePane = _dockingManager.Layout.BottomSide.Children.FirstOrDefault();
                    break;
                case AnchorableShowStrategy.Top:
                    anchorablePane = _dockingManager.Layout.TopSide.Children.FirstOrDefault();
                    break;
            }

            if (anchorablePane == null)
            {
                anchorablePane = new LayoutAnchorGroup();
                switch (strategy)
                {
                    case AnchorableShowStrategy.Left:
                        _dockingManager.Layout.LeftSide.Children.Add(anchorablePane);
                        break;
                    case AnchorableShowStrategy.Right:
                        _dockingManager.Layout.RightSide.Children.Add(anchorablePane);
                        break;
                    case AnchorableShowStrategy.Bottom:
                        _dockingManager.Layout.BottomSide.Children.Add(anchorablePane);
                        break;
                    case AnchorableShowStrategy.Top:
                        _dockingManager.Layout.TopSide.Children.Add(anchorablePane);
                        break;
                }
            }

            anchorablePane.Children.Add(anchorable);
            anchorable.IsSelected = state;
            anchorable.IsActive = state;
            anchorable.Show();
        }

        public void ShowAnchorable(UserControl view, string title, AnchorableShowStrategy strategy = AnchorableShowStrategy.Left)
        {
            //var anchorable = new LayoutAnchorable
            //{
            //    Title = title,
            //    Content = view,
            //    CanClose = false,
            //    CanHide = true
            //};

            //LayoutAnchorablePane anchorablePane = null;

            //switch (strategy)
            //{
            //    case AnchorableShowStrategy.Left:
            //        anchorablePane = _dockingManager.Layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(p => p.Parent == _dockingManager.Layout.LeftSide);
            //        if (anchorablePane == null)
            //        {
            //            anchorablePane = new LayoutAnchorablePane();
            //            _dockingManager.Layout.LeftSide.Children.Add(anchorablePane);
            //        }
            //        break;
            //    case AnchorableShowStrategy.Right:
            //        anchorablePane = _dockingManager.Layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(p => p.Parent == _dockingManager.Layout.RightSide);
            //        if (anchorablePane == null)
            //        {
            //            anchorablePane = new LayoutAnchorablePane();
            //            _dockingManager.Layout.RightSide.Children.Add(anchorablePane);
            //        }
            //        break;
            //    case AnchorableShowStrategy.Bottom:
            //        anchorablePane = _dockingManager.Layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(p => p.Parent == _dockingManager.Layout.BottomSide);
            //        if (anchorablePane == null)
            //        {
            //            anchorablePane = new LayoutAnchorablePane();
            //            _dockingManager.Layout.BottomSide.Children.Add(anchorablePane);
            //        }
            //        break;
            //    case AnchorableShowStrategy.Top:
            //        anchorablePane = _dockingManager.Layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(p => p.Parent == _dockingManager.Layout.TopSide);
            //        if (anchorablePane == null)
            //        {
            //            anchorablePane = new LayoutAnchorablePane();
            //            _dockingManager.Layout.TopSide.Children.Add(anchorablePane);
            //        }
            //        break;
            //}

            //anchorablePane.Children.Add(anchorable);

            //anchorable.IsActive = true;
            //anchorable.IsSelected = true;
            //anchorable.AddToLayout(_dockingManager, false);
        }

        // --- существующие методы ShowDocument / AddDocumentTab и т.д. ---

        public ITabPanelContent GetActiveDirectoryPanel()
        {
            if (_dockingManager?.ActiveContent is not LayoutContent layout)
                return null;

            if (layout.Content is not ContentControl cc)
                return null;

            if (cc.Content is not FrameworkElement fe)
                return null;

            return fe.DataContext as ITabPanelContent;
        }

        public string GetActiveTabPath()
        {
            if (GetActiveDirectoryPanel() is ITabPanelContent directoryPanel)
                return directoryPanel.GetCurrentPath();
            throw new ArgumentNullException();
        }
    }
}
