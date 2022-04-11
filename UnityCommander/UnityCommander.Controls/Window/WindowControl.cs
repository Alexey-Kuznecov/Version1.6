
namespace UnityCommander.Controls.Window
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// This class defines the attached property and related change handler that calls the WindowUnityStyle.
    /// </summary>
    public static class WindowControl
    {
        #region Dependency properties

        /// <summary>
        /// The WindowInstance attached property.
        /// </summary>
        private static readonly DependencyProperty WindowInstanceProperty = DependencyProperty.RegisterAttached(
            "WindowInstance",
            typeof(Window),
            typeof(WindowControl),
            new PropertyMetadata(defaultValue: null, propertyChangedCallback: WindowInstanceChanged));

        /// <summary>
        /// The WindowInstance attached property.
        /// </summary>
        private static readonly DependencyProperty ViewModelProperty = DependencyProperty.RegisterAttached(
            "ViewModel",
            typeof(CustomViewModel),
            typeof(WindowControl),
            new PropertyMetadata(defaultValue: null));

        #endregion

        /// <summary>
        /// The window this view model controls
        /// </summary>
        private static Window mainWindow;

        #region Getter/Setter methods
        
        /// <summary>
        /// Gets the value for the <see cref="WindowInstanceProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <returns>The <see cref="WindowInstanceProperty"/> attached to the <paramref name="obj"/> element.</returns>
        public static Window GetWindowInstance(DependencyObject obj)
        {
            return (Window)obj.GetValue(WindowInstanceProperty);
        }

        /// <summary>
        /// Sets the <see cref="WindowInstanceProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <param name="value">The value to attach.</param>
        public static void SetWindowInstance(DependencyObject obj, Window value)
        {
            obj.SetValue(WindowInstanceProperty, value);
        }

        /// <summary>
        /// Gets the value for the <see cref="WindowInstanceProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <returns>The <see cref="WindowInstanceProperty"/> attached to the <paramref name="obj"/> element.</returns>
        public static CustomViewModel GetViewModel(DependencyObject obj)
        {
            return (CustomViewModel)obj.GetValue(ViewModelProperty);
        }

        /// <summary>
        /// Sets the <see cref="WindowInstanceProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <param name="value">The value to attach.</param>
        public static void SetViewModel(DependencyObject obj, CustomViewModel value)
        {
            obj.SetValue(ViewModelProperty, value);
        }

        #endregion

        /// <summary>
        /// The window instance changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void WindowInstanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(d))
            {
                mainWindow = (Window)e.NewValue;
                SetViewModel(mainWindow, new CustomViewModel(mainWindow));

                var template = (ControlTemplate)Application.Current.FindResource("ControlTemplate");

                if (template != null)
                {
                    var winStyles = new Style();
                    winStyles.Setters.Add(new Setter(Control.TemplateProperty, template));
                    
                    if (mainWindow != null)
                    {
                        mainWindow.Style = winStyles;
                    }
                }
            }
        }
    }
}