
using AvalonDock;
using AvalonDock.Core;
using AvalonDock.Serializer.Xml;
using System;
using System.Diagnostics;
using System.IO;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Docking;

namespace UnityCommander.Services.Docking
{
    public sealed class ToolDockingService : IToolDockingService
    {
        private const string LayoutPath = "tool-layout.xml";

        private readonly IToolRegistry _toolRegistry;

        private DockingManager? _dockingManager;

        public ToolDockingService(IToolRegistry toolRegistry)
        {
            _toolRegistry = toolRegistry;
        }

        public void SetDockingManager(DockingManager manager)
        {
            _dockingManager = manager;
        }

        public void Load()
        {
            if (_dockingManager == null)
                throw new InvalidOperationException(
                    "DockingManager has not been initialized.");

            if (!File.Exists(LayoutPath))
                return;

            try
            {
                var serializer = CreateSerializer();

                using var reader = new StreamReader(LayoutPath);
                serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Tool layout load failed: {ex}");

                // Битый layout не должен ломать запуск приложения.
                try
                {
                    File.Delete(LayoutPath);
                }
                catch
                {
                    // Ничего.
                }
            }
        }

        public void Save()
        {
            if (_dockingManager == null)
                throw new InvalidOperationException(
                    "DockingManager has not been initialized.");

            var serializer = new XmlLayoutSerializer(_dockingManager);

            using var writer = new StreamWriter(LayoutPath);
            serializer.Serialize(writer);
        }

        private XmlLayoutSerializer CreateSerializer()
        {
            var serializer = new XmlLayoutSerializer(_dockingManager);

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
                Debug.WriteLine(
                    $"Tool descriptor not found: {contentId}");

                return;
            }

            args.Content = descriptor.Create();
        }
    }
}
