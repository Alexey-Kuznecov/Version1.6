
namespace WindowCustomizer
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Shell;

    /// <summary>
    /// The window control.
    /// </summary>
    public static class WindowControl
    {
        #region Declaration dependency properties

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
        private static readonly DependencyProperty OverrideStylesProperty = DependencyProperty.RegisterAttached(
            "OverrideStyles",
            typeof(Style),
            typeof(WindowControl),
            new PropertyMetadata(defaultValue: null));

        #endregion

        /// <summary>
        /// The window this view model controls
        /// </summary>
        private static Window mainWindow;

        /// <summary>
        /// The view model.
        /// </summary>
        private static Configuration viewModel;

        #region Getter/Setter Methods

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
        /// Gets the value for the <see cref="OverrideStylesProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <returns>The <see cref="OverrideStylesProperty"/> attached to the <paramref name="obj"/> element.</returns>
        public static Style GetConfiguration(DependencyObject obj)
        {
            return (Style)obj.GetValue(OverrideStylesProperty);
        }

        /// <summary>
        /// Sets the <see cref="OverrideStylesProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <param name="value">The value to attach.</param>
        public static void SetConfiguration(DependencyObject obj, Style value)
        {
            obj.SetValue(OverrideStylesProperty, value);
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
                viewModel = new Configuration(mainWindow);
                var template = (ControlTemplate)Application.Current.FindResource("ControlTemplate");
                
                if (template != null)
                {
                    var border = (Border)template.LoadContent();
                    border.DataContext = viewModel;
                    var winStyles = new Style();
                    winStyles.Setters.Add(new Setter(Control.TemplateProperty, template));
                    winStyles.TargetType = mainWindow.GetType();
                    mainWindow.Style = winStyles;
                }

                WindowChrome chrome = new WindowChrome();
                
                /*
                Binding bind = new Binding
                {
                    Source = viewModel,
                    Path = new PropertyPath(nameof(viewModel.ResizeBorderThickness))
                };
                Binding bind2 = new Binding
                {
                    Source = viewModel,
                    Path = new PropertyPath(nameof(viewModel.TitleHeight))
                };
                */
                mainWindow.SetValue(WindowChrome.CornerRadiusProperty, new CornerRadius(0));
                mainWindow.SetValue(WindowChrome.GlassFrameThicknessProperty, new Thickness(0));
                mainWindow.SetValue(WindowChrome.ResizeBorderThicknessProperty, new Thickness(30)); // bind);
                mainWindow.SetValue(WindowChrome.CaptionHeightProperty, 46.1);
                WindowChrome.SetWindowChrome(mainWindow, chrome);
            }
        }

        /// <summary>
        /// The configuration changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void ConfigurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

    }
}
