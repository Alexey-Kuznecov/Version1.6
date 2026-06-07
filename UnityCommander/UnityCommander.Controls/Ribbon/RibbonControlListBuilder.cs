
namespace UnityCommander.Controls.Ribbon
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using UnityCommander.Common.Models.Icons;
    using UnityCommander.Controls.Ribbon.Control;
    using UnityCommander.Controls.Ribbon.Subgroup;

    [SuppressMessage("ReSharper", "ConvertToUsingDeclaration")]
    public class RibbonControlListBuilder : IDisposable
    {
        private readonly List<UIElement> itemCollection = new ();
        private readonly List<DataBindingControl> dataBindingControls = new ();
        private readonly List<RibbonComboBox> comboBoxes = new ();
        public ControlsStackGroup ControlsStackGroup { get; } = new ();
        public void AddItem(Action<RibbonControlListBuilder> item) => item(this);
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
        public void AddDropItem(string content, IIcon icon, Action<RibbonDropListBuilder> callback)
        {
            var dropList = new RibbonDropListBuilder(content, icon);
            callback(dropList);
            this.itemCollection.Add(dropList.DropListItem);
        }
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
