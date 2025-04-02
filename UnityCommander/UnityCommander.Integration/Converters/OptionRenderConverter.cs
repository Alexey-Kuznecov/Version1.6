
namespace UnityCommander.Integration.Converters
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using UnityCommander.Common;
    using UnityCommander.Integration.Options;

    using Selector = System.Windows.Controls.Primitives.Selector;

    /// <summary>
    /// The option render converter.
    /// </summary>
    public class OptionRenderConverter : BaseConverter<OptionRenderConverter>
    {
        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IOption opt && opt.Render == OptionRender.DropBox)
            {
                ItemsControl itemsControl = new ItemsControl();
                var comboBox = this.CreateRenderComboBox(opt);
                var grid = this.CreateGrid();
                grid.AppendChild(comboBox);
                DataTemplate cellTemplate = new DataTemplate();
                cellTemplate.VisualTree = grid;
                itemsControl.ItemTemplate = cellTemplate;
                return itemsControl;
            }

            return value?.ToString();
        }

        private FrameworkElementFactory CreateGrid()
        {
            FrameworkElementFactory gridFactory = new FrameworkElementFactory(typeof(Grid));
            //FrameworkElementFactory gridRowFactory = new FrameworkElementFactory(typeof(RowDefinition));
            //gridRowFactory.SetValue(RowDefinition.HeightProperty, new GridLength(1, GridUnitType.Star));
            
            //FrameworkElementFactory gridRowFactory2 = new FrameworkElementFactory(typeof(RowDefinition));
            //gridRowFactory2.SetValue(RowDefinition.HeightProperty, new GridLength(1, GridUnitType.Star));
            
            ////FrameworkElementFactory colDefinitions = new FrameworkElementFactory(typeof(RowDefinitionCollection));
            //gridFactory.AppendChild(gridRowFactory);
            //gridFactory.AppendChild(gridRowFactory2);

            return gridFactory;
        }

        /// <summary>
        /// The create render combo box.
        /// </summary>
        /// <param name="opt">
        /// The opt.
        /// </param>
        /// <returns>
        /// The <see cref="ComboBox"/>.
        /// </returns>
        private FrameworkElementFactory CreateRenderComboBox(IOption opt)
        {
            var pluginOptVal = opt.Option;
            var source = (IEnumerable)pluginOptVal;

            if (source == null) return null;
            FrameworkElementFactory comboBoxFactory = new FrameworkElementFactory(typeof(ComboBox));
            comboBoxFactory.SetValue(FrameworkElement.WidthProperty, 200.0);
            comboBoxFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 10, 0, 10));
            comboBoxFactory.SetValue(ItemsControl.ItemTemplateProperty, (DataTemplate)Application.Current.FindResource("CombinedTemplate"));
            comboBoxFactory.SetValue(ItemsControl.ItemsSourceProperty, source);
            comboBoxFactory.SetValue(Selector.SelectedItemProperty, opt.DefaultOption);
            comboBoxFactory.AddHandler(Selector.SelectionChangedEvent, new SelectionChangedEventHandler(this.ComboBox_OnSelectionChanged));
            return comboBoxFactory;
        }


        /// <summary>
        /// The combo box_ on selection changed.
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
