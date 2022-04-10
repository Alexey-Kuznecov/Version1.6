
namespace UnityCommander.Controls.Ribbon
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    using UnityCommander.Controls.Ribbon.Control;

    /// <summary>
    /// The ribbon item group.
    /// </summary>
    public class RibbonItemGroup : StackPanel
    {
        /// <summary>
        /// The item collection.
        /// </summary>
        private readonly List<UIElement> itemCollection = new ();

        /// <summary>
        /// The ribbon control models.
        /// </summary>
        private readonly List<RibbonControlModel> ribbonControlModels = new ();

        /// <summary>
        /// The combo box.
        /// </summary>
        private readonly ComboBox comboBox = new ();

        /// <summary>
        /// The add item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void AddItem(Action<RibbonItemGroup> item) => item(this);

        /// <summary>
        /// The add item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void AddItem(IRibbonControl item)
        {
            switch (item)
            {
                case RibbonListBox:
                    {
                        ListBoxItem listBoxItem = new ()
                        {
                            DataContext = item.DataContext,
                            Template = item.Template,
                            Style = item.Style
                        };

                        this.itemCollection.Add(listBoxItem);
                        break;
                    }

                case RibbonComboBox:
                    this.ribbonControlModels.Add(item.DataContext);
                    break;
            }
        }

        /// <summary>
        /// The build.
        /// </summary>
        public void Build()
        {
            if (this.ribbonControlModels.Count > 0)
            {
                this.comboBox.ItemTemplate = (DataTemplate)Application.Current.FindResource("RibbonComboBoxItemDataTemplate");
                this.comboBox.Style = (Style)Application.Current.FindResource("RibbonComboBoxStyle");
                this.comboBox.ItemsSource = this.ribbonControlModels;
                this.comboBox.SelectedIndex = 0;
                this.comboBox.SelectionChanged += ComboBox_SelectionChanged;
                this.itemCollection.Add(this.comboBox);
            }

            this.Margin = new Thickness(10, 5, 0, 0);
            foreach (var item in this.itemCollection)
            {
                this.Children.Add(item);
            }
        }

        /// <summary>
        /// The combo box_ selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = e.AddedItems[0] as RibbonControlModel;

            selected?.Command?.Execute(null);
        }
    }
}
