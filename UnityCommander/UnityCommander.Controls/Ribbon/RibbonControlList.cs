
namespace UnityCommander.Controls.Ribbon
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using UnityCommander.Controls.Ribbon.Control;

    /// <summary>
    /// Adds a list of controls below each other..
    /// </summary>
    [SuppressMessage("ReSharper", "ConvertToUsingDeclaration")]
    public class RibbonControlList : StackPanel, IDisposable
    {
        /// <summary>
        /// The item collection.
        /// </summary>
        private readonly List<UIElement> itemCollection = new ();

        /// <summary>
        /// The ribbon control models.
        /// </summary>
        private readonly List<DataBindingControl> dataBindingControls = new ();

        /// <summary>
        /// The combo box.
        /// </summary>
        private readonly List<RibbonComboBox> comboBoxes = new ();

        /// <summary>
        /// The add item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void AddItem(Action<RibbonControlList> item) => item(this);

        /// <summary>
        /// The add item.
        /// </summary>
        /// <param name="control">
        /// The item.
        /// </param>
        public void AddItem(IRibbonControl control)
        {
            ListBoxItem listBoxItem = new ()
            {
                DataContext = control.DataBinding,
                Template = control.Template,
                Style = control.Style
            };
            
            this.itemCollection.Add(listBoxItem);         
        }

        /// <summary>
        /// The add combo box item.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public void AddComboBoxItem(Action<RibbonComboBox> callback)
        {
            using (var comboBox = new RibbonComboBox())
            {
                callback(comboBox);
                comboBox.Build();

                if (comboBox.Items.Count > 0)
                {
                    this.itemCollection.Add(comboBox);
                }
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.comboBoxes.Clear();
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
            var selected = e.AddedItems[0] as DataBindingControl;

            selected?.Command?.Execute(null);
        }
    }
}
