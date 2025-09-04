
using Prism.Mvvm;
using Prism.Regions;
using System.Windows.Controls;
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
    }
}
