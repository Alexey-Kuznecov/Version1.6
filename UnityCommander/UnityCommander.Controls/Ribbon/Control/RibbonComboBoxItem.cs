using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace UnityCommander.Controls.Ribbon
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Data;
    using UnityCommander.Controls.Ribbon.Control;

    public class RibbonComboBoxItemGroup : ComboBox
    {
        private List<ContentControl> itemCollection = new ();

        private ListBoxItem contentControl;

        public void AddItem(Action<RibbonComboBoxItemGroup> item) => item(this);

        public void AddItem(IRibbonControl item)
        {
            var li = new ListBoxItem();
            this.contentControl = new ();
            this.contentControl.DataContext = item.DataContext;
            this.contentControl.Template = item.Template;
            this.contentControl.Style = item.Style;
            this.itemCollection.Add(this.contentControl);
        }

        //public void Build()
        //{
        //    this.Margin = new Thickness(0, 3, 0, 0);
        //    foreach (var item in itemCollection)
        //    {
        //        this.Children.Add(item);
        //    }
        //}
    }
}
