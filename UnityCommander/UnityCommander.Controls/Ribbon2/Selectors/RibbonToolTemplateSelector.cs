
using System.Windows;
using System.Windows.Controls;
using UnityCommander.Controls.Ribbon2.Models;

namespace UnityCommander.Controls.Ribbon2.Selectors
{
    public class RibbonToolTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ButtonTemplate { get; set; }
        public DataTemplate ToggleTemplate { get; set; }
        // other templates

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is RibbonToolModel model)
            {
                return model.Kind switch
                {
                    RibbonToolKind.Button => ButtonTemplate,
                    RibbonToolKind.Toggle => ToggleTemplate,
                    _ => ButtonTemplate,
                };
            }
            return base.SelectTemplate(item, container);
        }
    }
}
