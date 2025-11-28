
using System;
using System.Windows.Controls;
using UnityCommander.Common.Module;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace UnityCommander.Services.Interfaces
{
    //public enum AnchorableShowStrategy
    //{
    //    Left,
    //    Right,
    //    Bottom,
    //    Top
    //}

    public interface IDockingService
    {
        public DockingManager GetDockingManager();
        void ShowDocument(UserControl view, string title);
        void AddDocumentTab(string title, string regionName);
        void AddActiveDocumentTab(string title, string regionName);
        void ShowAnchorable(UserControl view, string title, bool state, AnchorableShowStrategy strategy = AnchorableShowStrategy.Left);
        void ShowAnchorable(UserControl view, string title, AnchorableShowStrategy strategy = AnchorableShowStrategy.Left);

        // --- New API ---
        /// <summary>Возвращает DataContext активного содержимого (ViewModel) или null.</summary>
        public ITabPanelContent GetActiveDirectoryPanel();

        /// <summary>Если DataContext реализует ITabPanelContent — возвращает текущий путь, иначе null.</summary>
        string? GetActiveTabPath();

        /// <summary>Событие, когда активное содержимое изменилось (ActiveContentChanged в AvalonDock).</summary>
        event EventHandler? ActiveContentChanged;
    }
}
