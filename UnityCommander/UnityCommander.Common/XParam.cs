using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace UnityCommander.Common
{
    using Models;

    public class XParam : IDisposable
    {
        #region IsSortcutEnabledProperty

        /// <summary>
        /// Gets or Sets whether the control can be used as drag source.
        /// </summary>
        public static readonly DependencyProperty IsIncludeProperty
            = DependencyProperty.RegisterAttached("IsInclude",
                                                  typeof(bool),
                                                  typeof(XParam),
                                                  new UIPropertyMetadata(false, IsIncludeChanged));

        /// <summary>
        /// Gets whether the control can be used as drag source.
        /// </summary>
        public static bool GetIsInclude(UIElement target)
        {
            return (bool)target.GetValue(IsIncludeProperty);
        }

        /// <summary>
        /// Sets whether the control can be used as drag source.
        /// </summary>
        public static void SetIsInclude(UIElement target, bool value)
        {
            target.SetValue(IsIncludeProperty, value);
        }

        private static void IsIncludeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Button bt)
            {
                bt.Click += Bt_Click;
                bt.Initialized += Bt_Initialized;
            }
            if (d is MenuItem mi)
            {
                mi.Click += Bt_Click;
                Initialized(d);
            }
        }

        private static void Bt_Initialized(object sender, EventArgs e)
        {
            Initialized(sender);
        }

        private static void Initialized(object uIElement)
        {
            var contentControl = uIElement as Button;
            var menuItem = uIElement as MenuItem;
            var dd = contentControl?.Content ?? menuItem?.Header;

            if (dd is XArray array)
            {
                if (uIElement is Button ct)
                    ct.Content = null;
                if (uIElement is MenuItem mi)
                    mi.Header = null;

                var grid = new Grid();
                var text = new TextBlock();
                text.Text = (string)array.GetValue(XArray.ContentProperty);

                grid.Children.Add(array);
                grid.Children.Add(text);

                if (uIElement is Button ct2)
                    ct2.Content = grid;
                if (uIElement is MenuItem mi2)
                    mi2.Header = grid;
            }
        }

        private static void Bt_Click(object sender, RoutedEventArgs e)
        {
            var contentControl = sender as Button;
            var menu = sender as MenuItem;
            var arr = (contentControl?.Content as Grid)?.Children[0] ?? (menu?.Header as Grid)?.Children[0];

            if (arr is XArray array)
            {
                var arrParam = new object[array.Children.Count];

                for (int i = 0; i < array.Children.Count; i++)
                {
                    var para = (XValue)array.Children[i];
                    arrParam[i] = para.MyProperty;
                }

                if (contentControl != null)
                    contentControl.CommandParameter = arrParam;
                if (menu != null)
                    menu.CommandParameter = arrParam;
            }
        }

        private XArray xArray;

        public XParam()
        {
        }

        public XParam(object control)
        {
            this.xArray = new XArray();
        }

        /// <summary>
        /// Add parameters.
        /// </summary>
        /// <param name="contentControl">
        /// The content control.
        /// </param>
        /// <param name="control">
        /// The control.
        /// </param>
        /// <param name="paramModel">
        /// The view model property name.
        /// </param>
        public void AddParam(string contentControl, object control, XParamModel paramModel)
        {
            var xValue = new XValue();
            Binding bind = new Binding(paramModel.PropertyName) { Mode = BindingMode.Default, Source = paramModel.Instance };
            BindingOperations.SetBinding(xValue, XValue.ParamProperty, bind);
            this.xArray.SetValue(XArray.ContentProperty, contentControl);
            this.xArray.Children.Add(xValue);

            if (control is MenuItem mi)
            {
                if (!mi.Header.Equals(xArray)) 
                    mi.Header = xArray;
            }
        }

        public void ParamFinal(object control)
        {
            if (control is MenuItem mi)
                XParam.SetIsInclude(mi, true);
        }
        public void Dispose()
        {
        }

        #endregion
    }
}
