using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UnityCommander.Common.Models;

namespace UnityCommander.Core
{
    public class XParam : IDisposable
    {
        #region IsSortcutEnabledProperty

        /// <summary>
        /// Gets or Sets whether the control can be used as drag source.
        /// </summary>
        public static readonly DependencyProperty IsEncludeProperty
            = DependencyProperty.RegisterAttached("IsEnclude",
                                                  typeof(bool),
                                                  typeof(XParam),
                                                  new UIPropertyMetadata(false, IsEncludeChanged));

        /// <summary>
        /// Gets whether the control can be used as drag source.
        /// </summary>
        public static bool GetIsEnclude(UIElement target)
        {
            return (bool)target.GetValue(IsEncludeProperty);
        }

        /// <summary>
        /// Sets whether the control can be used as drag source.
        /// </summary>
        public static void SetIsEnclude(UIElement target, bool value)
        {
            target.SetValue(IsEncludeProperty, value);
        }

        private static void IsEncludeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
        /// Add paramerters.
        /// </summary>
        /// <param name="contentControl">
        /// The content control.
        /// </param>
        /// <param name="control">
        /// The control.
        /// </param>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        /// <param name="viewModelPropName">
        /// The view model property name.
        /// </param>
        public void AddParam(string contentControl, object control, object viewModel, string viewModelPropName)
        {
            var xValue = new XValue();
            Binding bind = new Binding(viewModelPropName) { Mode = BindingMode.Default, Source = viewModel };
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
                XParam.SetIsEnclude(mi, true);
        }
        public void Dispose()
        {
        }

        #endregion
    }
}
