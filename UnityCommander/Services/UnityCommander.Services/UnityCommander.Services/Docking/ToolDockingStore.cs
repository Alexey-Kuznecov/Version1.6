
using AvalonDock;
using AvalonDock.Core;
using AvalonDock.Serializer.Xml;
using System;
using System.Diagnostics;
using System.IO;
using UnityCommander.Common;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Docking;

namespace UnityCommander.Services.Docking
{
    public class ToolDockingStore : IToolDockingStore
    {
        private readonly string _layoutPath;
        private readonly IToolRegistry _toolRegistry;
        private readonly ToolDockingHost _dockingHost;
        private readonly DockingContext _context;

        public ToolDockingStore(
            DockingContext context,
            ToolDockingHost dockingHost,
            IToolRegistry toolRegistry, 
            UnityCommanderPath commanderPath)
        {
            _layoutPath = 
                Path.Combine(
                    commanderPath.DataDirectory, 
                    "tool-layout.xml");

            _context = context;
            _toolRegistry = toolRegistry;
            _dockingHost = dockingHost;
        }

        public void Load()
        {
            if (_context.ToolManager == null)
                throw new InvalidOperationException(
                    "DockingManager has not been initialized.");

            if (!File.Exists(_layoutPath))
                return;

            try
            {
                var serializer = CreateSerializer();

                using var reader = new StreamReader(_layoutPath);
                serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Tool layout load failed: {ex}");

                // Битый layout не должен ломать запуск приложения.
                try
                {
                    File.Delete(_layoutPath);
                }
                catch
                {
                    // Ничего.
                }
            }
        }

        public void Save()
        {
            if (_context.ToolManager == null)
                throw new InvalidOperationException(
                    "DockingManager has not been initialized.");

            var serializer = new XmlLayoutSerializer(_context.ToolManager);

            using var writer = new StreamWriter(_layoutPath);
            serializer.Serialize(writer);
        }

        private XmlLayoutSerializer CreateSerializer()
        {
            var serializer = new XmlLayoutSerializer(_context.ToolManager);

            serializer.LayoutSerializationCallback += OnLayoutItem;

            return serializer;
        }

        private void OnLayoutItem(
            object? sender,
            LayoutSerializationCallbackEventArgs args)
        {
            var contentId = args.Model.ContentId;

            if (string.IsNullOrWhiteSpace(contentId))
                return;

            var descriptor = _toolRegistry.Get(contentId);

            if (descriptor == null)
            {
                descriptor = _toolRegistry.FindByContentId(contentId);
            }

            if (descriptor == null)
            {
                Debug.WriteLine(
                    $"Tool descriptor not found: {contentId}");

                return;
            }
        
            args.Content = descriptor.Create();
        }
    }
}
