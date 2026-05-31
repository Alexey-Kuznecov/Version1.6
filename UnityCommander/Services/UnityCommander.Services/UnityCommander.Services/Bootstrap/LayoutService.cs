
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using UnityCommander.Common.Layout;
using UnityCommander.Common.State;
using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Bootstrap;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace UnityCommander.Services.Bootstrap
{
    public class LayoutService : ILayoutService
    {
        private readonly IDockingService _dockingService;
        private readonly ILayoutContentFactory _factory;
        private readonly ILogger _logger;
        private AppSessionState _session;

        public LayoutService(
            IDockingService dockingService, 
            ILayoutContentFactory factory,
             LoggerCreator logger)
        {
            var log = logger;
            _logger = log.Create(
               category: LogCategory.System,
               scope: LogScope.UI
            );

            _dockingService = dockingService;
            _factory = factory;
        }

        public void Load(AppSessionState session)
        {
            var path = "layout.xml";
            _session = session;
            if (!IsValidLayoutFile(path))
            { 
                CreateDefaultLayout(); 
                return;
            }

            try
            {
                LoadInternal(CreateSerializer(), path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Layout load failed: " + ex);
                File.Delete(path);
            }
        }

        private void LoadInternal(XmlLayoutSerializer serializer, string path)
        {
            using var reader = new StreamReader(path);
            serializer.Deserialize(reader);
        }

        private XmlLayoutSerializer CreateSerializer()
        {
            var manager = _dockingService.GetDockingManager();

            var serializer = new XmlLayoutSerializer(manager);
            serializer.LayoutSerializationCallback += OnLayoutItem;

            return serializer;
        }

        private void OnLayoutItem(object sender, LayoutSerializationCallbackEventArgs args)
        {
            var id = args.Model.ContentId;
            var content = new ContentControl();


            if (!Guid.TryParse(id, out var tabId))
            { 
                _logger.Warning($"Invalid ContentId: {id}");
                return;
            }

            var tab = _session.Panels
                .SelectMany(p => p.Tabs)
                .FirstOrDefault(t => t.TabId == tabId);

            if (tab == null)
            {
                _logger.Error("Tab not exist");
                return;
            }

            var path = tab?.Path ?? "C:\\";

            _factory.Create(content, tabId, path, vm =>
            {
                vm.TabTitleChanged += formatPath =>
                {
                    args.Model.Title = formatPath;
                };
            });

            args.Content = content;
        }

        public void CreateDefaultLayout()
        {
            var manager = _dockingService.GetDockingManager();

            var root = new LayoutRoot();
            var panel = new LayoutPanel { Orientation = Orientation.Horizontal };

            root.RootPanel = panel;

            foreach (var panelState in _session.Panels)
            {
                var docPane = new LayoutDocumentPane();

                panel.Children.Add(docPane);

                foreach (var tab in panelState.Tabs)
                {
                    var doc = new LayoutDocument
                    {
                        Title = tab.Path,
                        ContentId = panelState.PanelId.ToString()
                    };

                    docPane.Children.Add(doc);
                }
            }

            manager.Layout = root;
        }

        public bool IsValidLayoutFile(string path)
        {
            if (!File.Exists(path))
                return false;

            var text = File.ReadAllText(path);

            if (string.IsNullOrWhiteSpace(text))
                return false;

            return true;
        }

        public void Save()
        {
            var dock = _dockingService.GetDockingManager();
            var serializer = new XmlLayoutSerializer(_dockingService.GetDockingManager());

            using (var stream = new StreamWriter("layout.xml"))
            {
                serializer.Serialize(stream);
            }
        }
    }
}
