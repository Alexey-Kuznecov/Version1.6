
namespace WindowCustomizer
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Выполните шаги 1a или 1b, а затем 2, чтобы использовать этот пользовательский элемент управления в файле XAML.
    ///
    /// Шаг 1a. Использование пользовательского элемента управления в файле XAML, существующем в текущем проекте.
    /// Добавьте атрибут XmlNamespace в корневой элемент файла разметки, где он 
    /// будет использоваться:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WindowsCustomizer"
    ///
    ///
    /// Шаг 1б. Использование пользовательского элемента управления в файле XAML, существующем в другом проекте.
    /// Добавьте атрибут XmlNamespace в корневой элемент файла разметки, где он 
    /// будет использоваться:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WindowsCustomizer;assembly=WindowsCustomizer"
    ///
    /// Потребуется также добавить ссылку из проекта, в котором находится файл XAML,
    /// на данный проект и заново выполнить построение во избежание ошибок компиляции:
    ///
    ///     Щелкните правой кнопкой мыши нужный проект в обозревателе решений и выберите
    ///     "Добавить ссылку"->"Проекты"->[Выберите этот проект]
    ///
    ///
    /// Шаг 2)
    /// Продолжайте дальше и используйте элемент управления в файле XAML.
    ///
    ///     <MyNamespace:WindowControl/>
    ///
    /// </summary>
    public class WindowControl : Control
    {
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
        private static readonly DependencyProperty ConfigurationProperty = DependencyProperty.RegisterAttached(
            "WindowInstance",
            typeof(Configuration),
            typeof(WindowControl),
            new PropertyMetadata(defaultValue: null, propertyChangedCallback: ConfigurationChanged));

        /// <summary>
        /// The window this view model controls
        /// </summary>
        private static Window mainWindow;

        /// <summary>
        /// The view model.
        /// </summary>
        private static Configuration viewModel;

        /// <summary>
        /// Initializes static members of the <see cref="WindowControl"/> class.
        /// </summary>
        static WindowControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowControl), new FrameworkPropertyMetadata(typeof(WindowControl)));
        }

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
                mainWindow.DataContext = viewModel;
                Style style = (Style)Application.Current.FindResource("CustomWindowStyles");
                mainWindow.Style = style;

                /*
                    WindowChrome chrome = new WindowChrome();
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
                    mainWindow.SetValue(WindowChrome.CornerRadiusProperty, new CornerRadius(0));
                    mainWindow.SetValue(WindowChrome.GlassFrameThicknessProperty, new Thickness(0));
                    mainWindow.SetBinding(WindowChrome.ResizeBorderThicknessProperty, bind);
                    mainWindow.SetBinding(WindowChrome.CaptionHeightProperty, bind2);
                    WindowChrome.SetWindowChrome(mainWindow, chrome); `
                */
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
        /// <exception cref="NotImplementedException">
        /// </exception>
        private static void ConfigurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
