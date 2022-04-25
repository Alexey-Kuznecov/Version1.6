using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnityCommander.Common
{
    using Models;

    public class XValue : Panel
    {
        #region IsSortcutEnabledProperty

        /// <summary>
        /// Gets or Sets whether the control can be used as drag source.
        /// </summary>
        public static readonly DependencyProperty ParamProperty
            = DependencyProperty.RegisterAttached("Param",
                                                  typeof(object),
                                                  typeof(XValue),
                                                  new UIPropertyMetadata(null, ParamChanged));

        public object MyProperty { get; set; }

        /// <summary>
        /// Gets whether the control can be used as drag source.
        /// </summary>
        public static object GetParam(UIElement target)
        {
            return (object)target.GetValue(ParamProperty);
        }

        /// <summary>
        /// Sets whether the control can be used as drag source.
        /// </summary>
        public static void SetParam(UIElement target, object value)
        {
            target.SetValue(ParamProperty, value);
        }

        private static void ParamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var param = d as XValue;
            param.MyProperty = e.NewValue;
        }

        #endregion
    }
}
