using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using UnityCommander.Common.Module;
using UnityCommander.Services.Interfaces;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace UnityCommander
{
    public class DockingService : IDockingService
    {
        private DockingManager _dockingManager;

        public event EventHandler ActiveContentChanged;

        public DockingService(DockingManager dockingManager)
        {
            _dockingManager = dockingManager;
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

        public void AddDocumentTab(string title, string regionName)
        {
            var contentControl = new ContentControl();
            RegionManager.SetRegionName(contentControl, regionName);
            ViewModelLocator.SetAutoWireViewModel(contentControl, true);
           
            var document = new LayoutDocument
            {
                Title = title,
                Content = contentControl
            };

            _dockingManager.Layout.Descendents()
                .OfType<LayoutDocumentPane>()
                .First()
                .Children.Add(document);
        }

        public void AddDocumentTabInNewPane(string title, string regionName)
        {
            var contentControl = new ContentControl();
            RegionManager.SetRegionName(contentControl, regionName);
            ViewModelLocator.SetAutoWireViewModel(contentControl, true);

            var newDocument = new LayoutDocument
            {
                Title = title,
                Content = contentControl,
                ContentId = regionName // важно для восстановления
            };

            var newPane = new LayoutDocumentPane(newDocument);

            // Вставим новый LayoutDocumentPane справа от текущей панели
            var root = _dockingManager.Layout.RootPanel;

            if (root != null)
            {
                var newPaneGroup = new LayoutPanel
                {
                    Orientation = Orientation.Horizontal
                };

                // Добавим существующий контент влево, новый — вправо
                newPaneGroup.Children.Add(root.Children.First());
                newPaneGroup.Children.Add(newPane);

                root.Children.Clear();
                root.Children.Add(newPaneGroup);
            }
            else
            {
                // fallback: просто добавим в новый Layout
                _dockingManager.Layout.RootPanel = new LayoutPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children = { newPane }
                };
            }

            newDocument.IsActive = true;
        }

        public void AddActiveDocumentTab(string title, string regionName)
        {
            var contentControl = new ContentControl();
            RegionManager.SetRegionName(contentControl, regionName);
            ViewModelLocator.SetAutoWireViewModel(contentControl, true);

            var document = new LayoutDocument
            {
                Title = title,
                Content = contentControl
            };

            // 🔥 Подписка после загрузки (когда VM уже создана)
            contentControl.Loaded += (s, e) =>
            {
                if (GetActiveDirectoryPanel() is IDirectoryPanel panel)
                {
                    panel.PathChanged += path =>
                    {
                        document.Title = path; // обновляем вкладку
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
