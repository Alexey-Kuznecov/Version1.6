
namespace UnityCommander.Controls.Ribbon
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using UnityCommander.Controls.Ribbon.Control;
    using UnityCommander.Controls.Ribbon.Subgroup;

    /// <summary>
    /// Adds a list of controls below each other..
    /// </summary>
    [SuppressMessage("ReSharper", "ConvertToUsingDeclaration")]
    public class RibbonControlListBuilder : IDisposable
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

        public ControlsStackGroup ControlsStackGroup { get; } = new ();

        /// <summary>
        /// The add item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void AddItem(Action<RibbonControlListBuilder> item) => item(this);

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

            foreach (var item in this.itemCollection)
            {
                this.ControlsStackGroup.Children.Add(item);
            }
        }
    }
}
