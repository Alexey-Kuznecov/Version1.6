
using AvalonDock;
using AvalonDock.Layout;
using System;
using System.Collections.Generic;
using UnityCommander.Abstractions.Panels;

namespace UnityCommander.Services.Interfaces
{
    public interface IDockingService
    {
        event EventHandler? ActiveContentChanged;
        public DockingManager GetDockingManager();
        public ITabPanelContent GetActiveDirectoryPanel();
        
        void AddActiveDocumentTab(
            string contentId, 
            string title, 
            string regionName);

        void Activate(
            LayoutDocument document);

        string? GetActiveTabPath();

        IEnumerable<LayoutDocument> GetDocuments();

        LayoutDocument? FindDocument(Guid contentId);
    }
}
