
namespace UnityCommander.Integration.Converters
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    
    using UnityCommander.Integration.Options;

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
            if (value is IOption opt)
            {
                if (opt.Render == OptionRender.DropBox)
                {
                    var pluginOptVal = opt.Option;
                    var comboBoxItemsSource = (IEnumerable)pluginOptVal;

                    if (comboBoxItemsSource != null)
                    {
                        var comboBox = new ComboBox
                           {
                               ItemTemplate =
                                   (DataTemplate)Application.Current.FindResource("CombinedTemplate"),
                               Width = 200,
                               ItemsSource = comboBoxItemsSource,
                               SelectedItem = opt.DefaultOption,
                               Tag = opt
                           };
                        comboBox.SelectionChanged += this.ComboBox_OnSelectionChanged;
                        return comboBox;
                    }
                }
            }

            return value?.ToString();
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
