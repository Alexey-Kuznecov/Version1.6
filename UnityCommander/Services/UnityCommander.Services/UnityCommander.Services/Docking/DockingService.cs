
using AvalonDock;
using AvalonDock.Layout;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services.Docking
{
    public sealed class DockingService : IDockingService
    {
        private DockingManager? _dockingManager;

        public event EventHandler? ActiveContentChanged;

        public DockingService(DockingManager dockingManager)
        {
            _dockingManager = dockingManager;
        }

        public void AddActiveDocumentTab(string contentId, string title, string regionName)
        {
            var contentControl = new ContentControl();
            RegionManager.SetRegionName(contentControl, regionName);
            ViewModelLocator.SetAutoWireViewModel(contentControl, true);

            var document = new LayoutDocument
            {
                Title = title,
                Content = contentControl,
                ContentId = contentId
            };

            contentControl.Loaded += (s, e) =>
            {
                if (GetActiveDirectoryPanel() is IDirectoryPanel panel)
                {
                    panel.TabTitleChanged += formatPath =>
                    {
                        document.Title = formatPath;
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
                var firstPane = _dockingManager.Layout
                    .Descendents()
                    .OfType<LayoutDocumentPane>()
                    .FirstOrDefault();

                firstPane?.Children.Add(document);
                document.IsActive = true;
            }
        }

        public void SetDockingManager(
            DockingManager dockingManager)
        {
            _dockingManager = dockingManager;
        }

        public DockingManager GetDockingManager()
            => _dockingManager;

        public void Activate(LayoutDocument document)
        {
            document.IsActive = true;
        }

        public LayoutDocument? GetActiveDocument()
        {
            return _dockingManager?
                .Layout
                .ActiveContent as LayoutDocument;
        }

        public IEnumerable<LayoutDocument> GetDocuments()
        {
            return _dockingManager.Layout
                .Descendents()
                .OfType<LayoutDocument>();
        }

        public LayoutDocumentPane? GetActiveDocumentPane()
        {
            return GetActiveDocument()?.Parent
                as LayoutDocumentPane;
        }

        public ITabPanelContent? GetActiveDirectoryPanel()
        {
            if (_dockingManager?.ActiveContent
                is not LayoutContent layout)
            {
                return null;
            }

            if (layout.Content is not ContentControl cc)
                return null;

            if (cc.Content is not FrameworkElement fe)
                return null;

            return fe.DataContext
                as ITabPanelContent;
        }

        public string GetActiveTabPath()
        {
            if (GetActiveDirectoryPanel()
                is ITabPanelContent directoryPanel)
            {
                return directoryPanel.GetCurrentPath();
            }

            throw new InvalidOperationException();
        }

        public LayoutDocument FindDocument(Guid contentId)
        {
            var document = GetDocuments()
               .FirstOrDefault(x =>
                   Guid.TryParse(x.ContentId, out var id)
                   && id == contentId);

                return document;
        }
    }
}
