
using System.Windows;
using System.Windows.Controls;
using UnityCommander.Modules.FilePanel.ViewModels;

namespace UnityCommander.Modules.FilePanel.Selectors
{
    public sealed class ContextMenuItemTemplateSelector
       : DataTemplateSelector
    {
        public DataTemplate? MenuItemTemplate { get; set; }

        public DataTemplate? SeparatorTemplate { get; set; }

        public override DataTemplate SelectTemplate(
            object item,
            DependencyObject container)
        {
            if (item is MenuItemViewModel vm && vm.IsSeparator)
                return SeparatorTemplate!;

            return MenuItemTemplate!;
        }
    }
}
