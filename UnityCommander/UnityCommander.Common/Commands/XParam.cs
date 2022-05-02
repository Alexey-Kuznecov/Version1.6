
namespace UnityCommander.Common.Commands
{
    using System.Windows;
    using System.Windows.Controls;

    public class XParam : Panel
    {
        #region IsSortcutEnabledProperty

        /// <summary>
        /// Gets or Sets whether the control can be used as drag source.
        /// </summary>
        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.RegisterAttached("Value",
                                                  typeof(object),
                                                  typeof(XParam),
                                                  new UIPropertyMetadata(null, ParamChanged));

        public object MyValue { get; set; }

        /// <summary>
        /// Gets whether the control can be used as drag source.
        /// </summary>
        public static object GetValue(UIElement target)
        {
            return (object)target.GetValue(ValueProperty);
        }

        /// <summary>
        /// Sets whether the control can be used as drag source.
        /// </summary>
        public static void SetValue(UIElement target, object value)
        {
            target.SetValue(ValueProperty, value);
        }

        private static void ParamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is XParam param) 
                param.MyValue = e.NewValue;
        }

        #endregion
    }
}
