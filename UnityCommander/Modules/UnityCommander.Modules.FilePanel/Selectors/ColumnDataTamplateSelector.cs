using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UnityCommander.Common.Models;

namespace UnityCommander.Modules.FilePanel.Selectors
{
    public class ColumnDataTamplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate sdsa = null;
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is FileModel)
            {
                sdsa = element.FindResource("ColumnNameDataTemplate") as DataTemplate;
                return sdsa;
            }
            else {
                sdsa = element.FindResource("ColumnNameDataTemplate") as DataTemplate;
                return sdsa;
            }

            return null;
        }
    }
}
