namespace UnityCommander.Common.Commands
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class MultiCommandParameter : IDisposable
    {
        #region IsSortcutEnabledProperty

        /// <summary>
        /// Gets or Sets whether the control can be used as drag source.
        /// </summary>
        public static readonly DependencyProperty IsIncludeProperty
            = DependencyProperty.RegisterAttached("IsInclude",
                                                  typeof(bool),
                                                  typeof(MultiCommandParameter),
                                                  new UIPropertyMetadata(false, IsIncludeChanged));


        private readonly XArray xArray;

        public MultiCommandParameter(object control)
        {
            this.xArray = new XArray();
        }

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
            switch (d)
            {
                case Button bt:
                    bt.Click += Bt_Click;
                    bt.Initialized += Bt_Initialized;
                    break;
                case MenuItem mi:
                    mi.Click += Bt_Click;
                    Initialized(d);
                    break;
            }
        }

        private static void Bt_Initialized(object sender, EventArgs e)
        {
            Initialized(sender);
        }

        private static void Initialized(object uIElement)
        {
            var bt = uIElement as Button;
            var mi = uIElement as MenuItem;
            var controlContent = bt?.Content ?? mi?.Header;

            if (controlContent is not XArray array) return;

            switch (uIElement)
            {
                case Button button:
                    button.Content = null;
                    break;
                case MenuItem menuItem:
                    menuItem.Header = null;
                    break;
            }

            var grid = new Grid();
            var text = new TextBlock
            {
                Text = (string)array.GetValue(XArray.ContentProperty)
            };

            grid.Children.Add(array);
            grid.Children.Add(text);

            switch (uIElement)
            {
                case Button button:
                    button.Content = grid;
                    break;
                case MenuItem menuItem:
                    menuItem.Header = grid;
                    break;
            }
        }

        private static void Bt_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var menu = sender as MenuItem;
            var uiElement = (button?.Content as Grid)?.Children[0] ?? (menu?.Header as Grid)?.Children[0];

            if (uiElement is XArray array)
            {
                var arrParam = new object[array.Children.Count];

                for (int i = 0; i < array.Children.Count; i++)
                {
                    var param = (XParam)array.Children[i];
                    arrParam[i] = param.MyValue;
                }

                if (button != null)
                    button.CommandParameter = arrParam;
                if (menu != null)
                    menu.CommandParameter = arrParam;
            }
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
        /// <param name="paramViewModel">
        /// The view model property name.
        /// </param>
        public void AddParam(string contentControl, object control, XParamViewModel paramViewModel)
        {
            var param = new XParam();
            Binding bind = new Binding(paramViewModel.PropertyName) { Mode = BindingMode.Default, Source = paramViewModel.Instance };
            BindingOperations.SetBinding(param, XParam.ValueProperty, bind);
            this.xArray.SetValue(XArray.ContentProperty, contentControl);
            this.xArray.Children.Add(param);

            if (control is MenuItem mi)
            {
                if (!mi.Header.Equals(this.xArray)) 
                    mi.Header = this.xArray;
            }
        }

        public void ParamFinal(object control)
        {
            if (control is MenuItem mi)
                MultiCommandParameter.SetIsInclude(mi, true);
        }
        public void Dispose()
        {
        }

        #endregion
    }
}
