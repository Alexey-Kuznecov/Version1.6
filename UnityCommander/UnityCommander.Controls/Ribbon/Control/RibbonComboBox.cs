
namespace UnityCommander.Controls.Ribbon.Control
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using UnityCommander.Common.Models.Icons;

    public class RibbonComboBox : ComboBox, IDisposable
    {
        private readonly List<RibbonComboBoxItem> comboBoxItem = new ();
        private readonly ComboBox comboBox = new ();
        public RibbonComboBox()
        {
            this.ItemTemplate = (DataTemplate)Application.Current.FindResource("RibbonComboBoxItemDataTemplate");
            this.Style = (Style)Application.Current.FindResource("RibbonComboBoxStyle");
        }

        public void AddItem(string text, IIcon icon, RibbonCommand command)
        {
            this.comboBoxItem.Add(new RibbonComboBoxItem(text, icon, command));
        }
        public void Dispose()
        {
        }
        public void Build()
        {
            this.ItemsSource = this.comboBoxItem.Select(d => d.DataBinding);
            this.SelectedIndex = 0;
            this.SelectionChanged += ComboBox_SelectionChanged;
        }
        private static void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                var selected = e.AddedItems[0] as DataBindingControl;
                selected?.GlobalCommand?.Command.Execute(null);
            }
        }
    }
}