using System;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Integration.Converters
{
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;

    using UnityCommander.Integration.Options;

    using Selector = System.Windows.Controls.Primitives.Selector;

    /// <summary>
    /// The option render selector.
    /// </summary>
    public class OptionRenderSelector : DataTemplateSelector
    {
        /// <summary>
        /// The select template.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The <see cref="DataTemplate"/>.
        /// </returns>
        public override DataTemplate SelectTemplate(object value, DependencyObject container)
        {
            if (value is IOption opt)
            {
                DataTemplate cellTemplate = new ();

                switch (opt.Render)
                {
                    case OptionRender.Default:
                    case OptionRender.DropBox:
                        {
                            var grid = this.CreateGrid();
                            var label = this.CreateTitle(opt);
                            var comboBox = this.CreateComboBox(opt);

                            if (comboBox != null)
                            {
                                grid.AppendChild(comboBox);
                                grid.AppendChild(label);
                                cellTemplate.VisualTree = grid;
                            }

                            return cellTemplate;
                        }
                    case OptionRender.TextField:
                    case OptionRender.TextBlock:
                    case OptionRender.Checkbox:
                        {
                            var checkBox = this.CreateCheckbox(opt);
                            cellTemplate.VisualTree = checkBox;
                            return cellTemplate;
                        }
                    default:
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// The create grid.
        /// </summary>
        /// <returns>
        /// The <see cref="FrameworkElementFactory"/>.
        /// </returns>
        private FrameworkElementFactory CreateGrid()
        {
            FrameworkElementFactory gridFactory = new FrameworkElementFactory(typeof(Grid));
            FrameworkElementFactory gridRowFactory = new FrameworkElementFactory(typeof(RowDefinition));
            gridRowFactory.SetValue(RowDefinition.HeightProperty, GridLength.Auto);

            FrameworkElementFactory gridRowFactory2 = new FrameworkElementFactory(typeof(RowDefinition));
            gridRowFactory2.SetValue(RowDefinition.HeightProperty, new GridLength(1d, GridUnitType.Star));
            gridFactory.AppendChild(gridRowFactory);
            gridFactory.AppendChild(gridRowFactory2);
            return gridFactory;
        }

        /// <summary>
        /// The create combo box.
        /// </summary>
        /// <param name="opt">
        /// The opt.
        /// </param>
        /// <returns>
        /// The <see cref="FrameworkElementFactory"/>.
        /// </returns>
        private FrameworkElementFactory CreateComboBox(IOption opt)
        {
            var pluginOptVal = opt.Option;
            var source = (IEnumerable)pluginOptVal;

            if (source == null) return null;

            FrameworkElementFactory comboBoxFactory = new FrameworkElementFactory(typeof(ComboBox));
            comboBoxFactory.SetValue(FrameworkElement.WidthProperty, 230d);
            comboBoxFactory.SetValue(Grid.RowProperty, 1);
            comboBoxFactory.SetValue(ItemsControl.ItemTemplateProperty, (DataTemplate)Application.Current.FindResource("CombinedTemplate"));
            comboBoxFactory.SetValue(ItemsControl.ItemsSourceProperty, source);
            comboBoxFactory.SetValue(FrameworkElement.TagProperty, opt);
            comboBoxFactory.SetValue(System.Windows.Controls.Primitives.Selector.SelectedItemProperty, opt.DefaultOption);
            comboBoxFactory.AddHandler(Selector.SelectionChangedEvent, new SelectionChangedEventHandler(this.ComboBox_OnSelectionChanged));
            return comboBoxFactory;
        }

        /// <summary>
        /// The create check box.
        /// </summary>
        /// <param name="opt">
        /// The opt.
        /// </param>
        /// <returns>
        /// The <see cref="FrameworkElementFactory"/>.
        /// </returns>
        private FrameworkElementFactory CreateCheckbox(IOption opt)
        {
            FrameworkElementFactory checkboxFactory  = new (typeof(CheckBox));
            FrameworkElementFactory TextBlockFactory = new (typeof(TextBlock));
            FrameworkElementFactory gridFactory      = new (typeof(Grid));
            FrameworkElementFactory ckColumnFactory  = new (typeof(ColumnDefinition));
            FrameworkElementFactory tbColumnFactory  = new (typeof(ColumnDefinition));

            // Checkbox
            checkboxFactory.SetValue(Grid.ColumnProperty, 1);
            checkboxFactory.SetValue(CheckBox.IsCheckedProperty, opt.Option);
            checkboxFactory.SetValue(CheckBox.TagProperty, opt);
            checkboxFactory.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(CheckBox_OnCheckedChanged));
            checkboxFactory.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(CheckBox_OnCheckedChanged));
            // TextBlock
            TextBlockFactory.SetValue(Grid.ColumnProperty, 0);
            TextBlockFactory.SetValue(TextBlock.TextProperty, opt.Title);
            TextBlockFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 5, 0, 5));
            TextBlockFactory.SetValue(TextBlock.FontStyleProperty, FontStyles.Italic);
            TextBlockFactory.SetValue(TextBlock.FontWeightProperty, FontWeights.Medium);
            // ColumnDefinition
            ckColumnFactory.SetValue(ColumnDefinition.WidthProperty, new GridLength(1d, GridUnitType.Star));
            tbColumnFactory.SetValue(ColumnDefinition.WidthProperty, new GridLength(25d));
            // Grid
            gridFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(0,15,0,0));
            gridFactory.AppendChild(checkboxFactory);
            gridFactory.AppendChild(TextBlockFactory);
            gridFactory.AppendChild(ckColumnFactory);
            gridFactory.AppendChild(tbColumnFactory);
            return gridFactory;
        }

        /// <summary>
        /// The create title.
        /// </summary>
        /// <param name="opt">
        /// The opt.
        /// </param>
        /// <returns>
        /// The <see cref="FrameworkElementFactory"/>.
        /// </returns>
        private FrameworkElementFactory CreateTitle(IOption opt)
        {
            FrameworkElementFactory textFactory = new FrameworkElementFactory(typeof(TextBlock));
            textFactory.SetValue(FrameworkElement.HeightProperty, 25d);
            textFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 5, 0, 5));
            textFactory.SetValue(TextBlock.TextProperty, opt.Title);
            textFactory.SetValue(TextBlock.FontStyleProperty, FontStyles.Italic);
            textFactory.SetValue(TextBlock.FontWeightProperty, FontWeights.Medium);
            textFactory.SetValue(Grid.RowProperty, 0);
            return textFactory;
        }

        /// <summary>
        /// The check box on checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckBox_OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkbox)
            {
                var isChecked = checkbox.IsChecked;

                if (checkbox.Tag is IOption option)
                {
                    option.Handler.DynamicInvoke(isChecked);
                }
            }
        }

        /// <summary>
        /// The combo box on selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                var selectItem = comboBox.SelectedItem;

                if (comboBox.Tag is IOption option)
                {
                    option.Handler.DynamicInvoke(selectItem);
                }
            }
        }
    }
}
