
namespace UnityCommander.Common.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using UnityCommander.Core.Mvvm.Converters;
    using UnityCommander.Integration.Contracts;

    /// <summary>
    /// The color converter solid color.
    /// </summary>
    public class ColorConverterSolidColor : BaseConverter<ColorConverterSolidColor>
    {
        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <param name="targetType"> The target type. </param>
        /// <param name="parameter"> The parameter. </param>
        /// <param name="culture"> The culture. </param>
        /// <returns> The <see cref="object"/>. </returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        /// <summary>
        /// The convert back.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <param name="targetType"> The target type. </param>
        /// <param name="parameter"> The parameter. </param>
        /// <param name="culture"> The culture. </param>
        /// <returns> The <see cref="object"/>. </returns>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// The scale converter.
    /// </summary>
    public class ScaleConverter : BaseConverter<ScaleConverter>
    {
        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        public double Scale { get; set; } = 12;

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <param name="targetType"> The target type. </param>
        /// <param name="parameter"> The parameter. </param>
        /// <param name="culture"> The culture. </param>
        /// <returns> The <see cref="object"/>. </returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                double num = double.Parse(value.ToString());
                return (num * (this.Scale / 100));
            }
            return null;
        }

        /// <summary>
        /// The convert back.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <param name="targetType"> The target type. </param>
        /// <param name="parameter"> The parameter. </param>
        /// <param name="culture"> The culture. </param>
        /// <returns> The <see cref="object"/>. </returns>
        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// The content converter.
    /// </summary>
    public class ContentConverter : BaseConverter<ContentConverter>
    {
        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <param name="targetType"> The target type. </param>
        /// <param name="parameter"> The parameter. </param>
        /// <param name="culture"> The culture. </param>
        /// <returns> The <see cref="object"/>. </returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PathGeometry pathGeometry = new PathGeometry();
            Path path = new Path { Data = new PathGeometry() };

            DrawingGroup draw = value is IIcon
                    ? ((IIcon)value).Brush.Drawing as DrawingGroup
                    : (value as DrawingBrush)?.Drawing as DrawingGroup;

            if (draw != null)
            {
                foreach (var drawing in draw.Children)
                {
                    var item = (GeometryDrawing)drawing;
                    pathGeometry.AddGeometry(item.Geometry);
                    path.Data = pathGeometry.Clone();
                    path.Stretch = Stretch.Uniform;
                    path.Fill = item.Brush;
                }
            }

            return path;
        }
    }
}
