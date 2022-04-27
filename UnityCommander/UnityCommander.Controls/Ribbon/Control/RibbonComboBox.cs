
namespace UnityCommander.Controls.Ribbon.Control
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using UnityCommander.Common.Models.Icons;
    using UnityCommander.Integration.Commands;

    /// <summary>
    /// The ribbon combo box item.
    /// </summary>
    public class RibbonComboBox : ComboBox, IDisposable
    {
        /// <summary>
        /// The combo box item.
        /// </summary>
        private readonly List<RibbonComboBoxItem> comboBoxItem = new ();

        /// <summary>
        /// The combo box.
        /// </summary>
        private readonly ComboBox comboBox = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonComboBox"/> class.
        /// </summary>
        public RibbonComboBox()
        {
            this.ItemTemplate = (DataTemplate)Application.Current.FindResource("RibbonComboBoxItemDataTemplate");
            this.Style = (Style)Application.Current.FindResource("RibbonComboBoxStyle");
        }

        /// <summary>
        /// The add item.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="icon">
        /// The icon.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        public void AddItem(string text, IIcon icon, GlobalCommand command)
        {
            this.comboBoxItem.Add(new RibbonComboBoxItem(text, icon, command));
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// The build.
        /// </summary>
        public void Build()
        {
            this.ItemsSource = this.comboBoxItem.Select(d => d.DataBinding);
            this.SelectedIndex = 0;
            this.SelectionChanged += ComboBox_SelectionChanged;
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
            if (!Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                var selected = e.AddedItems[0] as DataBindingControl;
                selected?.GlobalCommand?.Command.Execute(null);
            }
        }
    }
}