
namespace UnityCommander.Modules.Viewer.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Controls;
    using UnityCommander.Common.Styling.Converters;

    /// <summary>
    /// The date time converter.
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
            if (value is object[] list)
            {
                var combo = new ComboBox();
                combo.SelectedIndex = 1;
                combo.ItemsSource = list;
                return combo;
            }
            else if (value is string text)
            {
                var textBox = new TextBox();
                textBox.Text = text;
                return textBox;
            }

            return value;
        }
    }
}
