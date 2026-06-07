
using System;
using System.Windows.Controls;
using UnityCommander.Common.Module;

namespace UnityCommander.Common.Layout
{
    public interface ILayoutContentFactory
    {
        public void Create(ContentControl content, Guid tabId, string path, Action<ITabPanelContent> onReady);
    }
}
