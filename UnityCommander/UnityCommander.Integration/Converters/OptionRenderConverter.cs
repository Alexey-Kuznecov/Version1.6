
namespace UnityCommander.Integration.Converters
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;

    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The option render converter.
    /// </summary>
    public class OptionRenderConverter : BaseConverter<OptionRenderConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OptionBuilder { OptionRender: OptionRender.DropBox } opt)
            {
                var pluginOptVal = opt.GetList();
                ComboBox comboBox = new ComboBox();
                var grid = new FrameworkElementFactory(typeof(Grid));
                comboBox.Width = 250;
                var comboBoxItemsSource = (IEnumerable)pluginOptVal;

                foreach (var o in comboBoxItemsSource)
                {
                    comboBox.Items.Add(o);
                    comboBox.SelectedItem = o;
                }

                WrapPanel wrapPanel = new WrapPanel();
                wrapPanel.Children.Add(comboBox);
                wrapPanel.Margin = new Thickness(10);
                return wrapPanel;
            }

            return value?.ToString();
        }
    }
}
