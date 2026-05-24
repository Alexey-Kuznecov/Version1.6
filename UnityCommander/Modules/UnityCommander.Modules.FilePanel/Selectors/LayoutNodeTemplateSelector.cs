
using System.Windows;
using System.Windows.Controls;
using UnityCommander.Modules.FilePanel.Layout;

namespace UnityCommander.Modules.FilePanel.Selectors
{
    public class LayoutNodeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DirectoryView { get; set; }
        public DataTemplate FileView { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ContentNode node)
            {
                return node.Key switch
                {
                    "DirectoryView" => DirectoryView,
                    "FileView" => FileView,
                    _ => null
                };
            }

            return null;
        }
    }
}
