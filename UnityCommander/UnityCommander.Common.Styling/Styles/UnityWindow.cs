using System.ComponentModel;
using System.Windows.Media.Effects;
using System;
using System.Windows.Media.Imaging;

#if HAS_WINUI
using Windows.UI.Xaml;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
#endif

namespace UnityCommander.Common.Styling.Styles.Mvvm.Base
{
    /// <summary>
    /// This class defines the attached property and related change handler that calls the WindowUnityStyle in Prism.Mvvm.
    /// </summary>
    public static class WindowUnityStyle
    {
        /// <summary>
        /// The window this view model controls
        /// </summary>
        private static Window mWindow;

        private static UnityWindowViewModel viewModel;

        /// <summary>
        /// The WindowInstance attached property.
        /// </summary>
        public static DependencyProperty WindowInstanceProperty = DependencyProperty.RegisterAttached("WindowInstance", typeof(Window), typeof(WindowUnityStyle), new PropertyMetadata(defaultValue: null, propertyChangedCallback: WindowInstanceChanged));

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

        private static void WindowInstanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if !HAS_WINUI
            if (!DesignerProperties.GetIsInDesignMode(d))
#endif
            {
                mWindow = (Window)e.NewValue;
                viewModel = new UnityWindowViewModel(mWindow);
                mWindow.DataContext = viewModel;
                mWindow.Style = (Style)Application.Current.FindResource("UnityWindowStyle");
                
                //var style  = new Style(typeof(Window));
                //var grid   = new FrameworkElementFactory(typeof(Grid));       

                //ControlTemplate controlTemplate = new ControlTemplate(typeof(Window));

                //// Outer border with the dropshadow margin
                //var borderRoot = new FrameworkElementFactory(typeof(Border));
                //borderRoot.SetValue(Border.DataContextProperty, viewModel);
                //borderRoot.SetBinding(Border.PaddingProperty, Bind(nameof(viewModel.OuterMarginSizeThickness)));

                //// Opacity mask for corners on grid
                //var borderGrid = new FrameworkElementFactory(typeof(Border));
                //borderGrid.SetValue(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(150,166,199)));
                //borderGrid.SetValue(Border.CornerRadiusProperty, Bind(nameof(viewModel.WindowCornerRadius)));
                //borderGrid.SetValue(Border.NameProperty, "Container");
                //grid.AppendChild(borderGrid);

                //// Window border and dropshadown
                //var borderWin = new FrameworkElementFactory(typeof(Border));
                //borderWin.SetValue(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(255,255,255)));
                //borderWin.SetValue(Border.CornerRadiusProperty, Bind(nameof(viewModel.WindowCornerRadius)));
                //DropShadowBitmapEffect dropShadowEffect = new DropShadowBitmapEffect();
                //dropShadowEffect.Opacity = 0.2;
                //dropShadowEffect.ShadowDepth = 0;
                //borderWin.SetValue(Border.BitmapEffectProperty, dropShadowEffect);
                //grid.AppendChild(borderWin);

                //// Corner clipping
                //var gridChild = new FrameworkElementFactory(typeof(Grid));
                //// var visualBrush = new FrameworkElementFactory(typeof(VisualBrush));
                //// visualBrush.SetBinding(VisualBrush.VisualProperty, Bind(null, "Container"));

                //// The main window content
                //// gridChild.SetValue(Grid.OpacityMaskProperty, visualBrush);
                //var rowDefinition = new FrameworkElementFactory(typeof(RowDefinition));
                //var rowDefinition2 = new FrameworkElementFactory(typeof(RowDefinition));
                //var rowDefinition3 = new FrameworkElementFactory(typeof(RowDefinition));
                //rowDefinition.SetBinding(RowDefinition.HeightProperty, Bind(nameof(viewModel.TitleHeightGridLength)));
                //rowDefinition2.SetValue(RowDefinition.HeightProperty, new GridLength(0, GridUnitType.Star));
                //rowDefinition3.SetValue(RowDefinition.HeightProperty, new GridLength(0, GridUnitType.Auto));
                //gridChild.AppendChild(rowDefinition); // Title Bar
                //gridChild.AppendChild(rowDefinition2); // Drop shadow
                //gridChild.AppendChild(rowDefinition3); // Window Content


                //var gridChild2 = new FrameworkElementFactory(typeof(Grid));
                //gridChild2.SetValue(Grid.ColumnProperty, 0);
                //gridChild2.SetValue(Grid.ZIndexProperty, 1);
                //var rowDefinition4 = new FrameworkElementFactory(typeof(RowDefinition));
                //var rowDefinition5 = new FrameworkElementFactory(typeof(RowDefinition));
                //var rowDefinition6 = new FrameworkElementFactory(typeof(RowDefinition));
                //rowDefinition4.SetValue(RowDefinition.HeightProperty, new GridLength(0, GridUnitType.Auto));
                //rowDefinition5.SetValue(RowDefinition.HeightProperty, new GridLength(0, GridUnitType.Star));
                //rowDefinition6.SetValue(RowDefinition.HeightProperty, new GridLength(0, GridUnitType.Auto));
                //gridChild2.AppendChild(rowDefinition4); // Icon
                //gridChild2.AppendChild(rowDefinition5); // Title
                //gridChild2.AppendChild(rowDefinition6); // Windows Button

                //// Icon
                //var button = new FrameworkElementFactory(typeof(Button));
                //button.SetBinding(Button.CommandProperty, Bind(nameof(viewModel.MenuCommand)));
                //button.SetValue(Button.StyleProperty, (Style)Application.Current.FindResource("SystemIconButton"));
                //var image = new FrameworkElementFactory(typeof(Image));
                //image.SetValue(Image.SourceProperty, new BitmapImage(new Uri(@"/Images/Logo/logo-small.png", UriKind.Relative)));
                //gridChild2.AppendChild(button);

                //// Title
                //var viewBox = new FrameworkElementFactory(typeof(Viewbox));
                //viewBox.SetValue(Grid.ColumnProperty, 0);
                //viewBox.SetValue(Viewbox.MarginProperty, new Thickness(0));
                //var textbox = new FrameworkElementFactory(typeof(TextBlock));
                //textbox.SetValue(TextBlock.StyleProperty, (Style)Application.Current.FindResource("HeaderText"));
                //Binding bind = new Binding();
                //bind.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
                //bind.Path = new PropertyPath("Title");
                //textbox.SetBinding(TextBlock.TextProperty, bind);
                //viewBox.AppendChild(textbox);
                //gridChild2.AppendChild(viewBox);

                //// Window Buttons
                //var stackpanel = new FrameworkElementFactory(typeof(StackPanel));
                //stackpanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                //stackpanel.SetValue(Grid.ColumnProperty, 2);
                //var button2 = new FrameworkElementFactory(typeof(Button));
                //var button3 = new FrameworkElementFactory(typeof(Button));
                //var button4 = new FrameworkElementFactory(typeof(Button));
                //button2.SetValue(Button.ContentProperty, "_");
                //button2.SetValue(Button.StyleProperty, Application.Current.FindResource("WindowControlButton"));
                //button2.SetBinding(Button.CommandProperty, Bind(nameof(viewModel.MaximizeCommand)));
                //button3.SetValue(Button.ContentProperty, "[ ]");
                //button3.SetValue(Button.StyleProperty, Application.Current.FindResource("WindowControlButton"));
                //button3.SetBinding(Button.CommandProperty, Bind(nameof(viewModel.MaximizeCommand)));
                //button4.SetValue(Button.ContentProperty, "X");
                //button4.SetValue(Button.StyleProperty, Application.Current.FindResource("WindowControlButton"));
                //button4.SetBinding(Button.CommandProperty, Bind(nameof(viewModel.CloseCommand)));
                //stackpanel.AppendChild(button2);
                //stackpanel.AppendChild(button3);
                //stackpanel.AppendChild(button4);
                //gridChild2.AppendChild(stackpanel);

                //// Finally
                //gridChild.AppendChild(gridChild2);
                //grid.AppendChild(gridChild);
                //borderRoot.AppendChild(grid);
                //controlTemplate.VisualTree = borderRoot;
                //style.Setters.Add(new Setter() { Property = Control.TemplateProperty, Value = controlTemplate });
                //mWindow.Resources.Add("UnityWindowStyle", style);
            }
        }

        private static Binding Bind(string path, string name = "", BindingMode mode = BindingMode.Default)
        {
            Binding bind = new Binding();
            bind.Mode = mode;

            if (!string.IsNullOrEmpty(path))
            {
                bind.Source = viewModel;
                bind.Path = new PropertyPath(path);
            }
            else
            {
                bind.ElementName = name;
            }

            return bind;
        }
    }
}