
using System.Windows.Controls;
using UnityCommander.Abstractions.Panels;

namespace UnityCommander.WPF
{
    public interface ILayoutContentFactory
    {
        public void Create(ContentControl content, Guid tabId, Guid panelId, string path, Action<ITabPanelContent> onReady);
    }
}
