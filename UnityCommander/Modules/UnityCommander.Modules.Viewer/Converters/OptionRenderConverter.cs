
namespace UnityCommander.Modules.Viewer.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using UnityCommander.Common.Styling.Converters;

    /// <summary>
    /// The date time converter.
    /// </summary>
    public class OptionRenderConverter : BaseConverter<OptionRenderConverter>
    {
        private int _itemIndex;

        private static ListBox lastContainer;

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
                Binding b = new Binding("DataContext.SelectedOption");
                b.Mode = BindingMode.TwoWay;
                b.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ListBox), 1);
                var combo = new ComboBox();
                combo.ItemsSource = list;
                BindingOperations.SetBinding(combo, ComboBox.SelectedItemProperty, b);
                combo.SelectedIndex = 1;
                combo.SelectionChanged += Combo_SelectionChanged;
                combo.PreviewMouseDown += PreviewMouseDown;
                combo.Tag = _itemIndex++;

                return combo;
            }
            else if (value is string text)
            {
                Binding b = new Binding("DataContext.SelectedOption");
                b.Mode = BindingMode.OneWayToSource;
                b.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ListBox), 1);
                var textBox = new TextBox();
                BindingOperations.SetBinding(textBox, TextBox.TextProperty, b);
                textBox.Text = text;
                textBox.PreviewMouseDown += PreviewMouseDown;
                textBox.Tag = _itemIndex++;
                return textBox;
            }

            return value;
        }

        private void PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Control lb = (Control)sender;
            lastContainer = this.GetContainer<ListBox>(lb, e);

            if (lastContainer != null)
                lastContainer.SelectedIndex = (int)lb.Tag;
        }

        /// <summary>
        /// The select list box item.
        /// </summary>
        /// <typeparam name="T"> Object dependency </typeparam>
        /// <param name="lb"> The container. </param>
        /// <param name="e"> The e.  </param>
        /// <returns> The <see cref="bool"/>. </returns>
        private T GetContainer<T>(Control lb, MouseEventArgs e) where T : DependencyObject
        {
            var point = e.GetPosition(lb);
            var o = VisualTreeHelper.HitTest(lb, point)?.VisualHit;

            if (o == null)
                return null;

            while (o is not ListBox)
            {
                if (o is T checkContainer)
                    return checkContainer;

                o = VisualTreeHelper.GetParent(o ?? throw new InvalidOperationException());
            }

            return (T)o;
        }

        private void Combo_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Binding b2 = new Binding("DataContext.ListOptions");
            //b2.Mode = BindingMode.OneWayToSource;
            //b2.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ListBox), 1);
            //BindingOperations.SetBinding(combo, ComboBox.TagProperty, b2);
            //combo.Tag = combo;

            //throw new NotImplementedException();
        }
    }
}
