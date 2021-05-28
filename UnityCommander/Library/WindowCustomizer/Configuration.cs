
namespace WindowCustomizer
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;

    using CustomWindow;

    using Prism.Commands;
    using Prism.Mvvm;

    /// <summary>
    /// The View Model for the custom flat window
    /// </summary>
    public class Configuration : BindableBase
    {
        #region Private Member

        /// <summary>
        /// The window this view model controls
        /// </summary>
        private readonly Window window;

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        private int outerMarginSize = 10;

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        private int windowRadius = 10;

        /// <summary>
        /// The resize border thickness.
        /// </summary>
        private Thickness resizeBorderThickness;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition dockPosition = WindowDockPosition.Undocked;

        /// <summary>
        /// The data context.
        /// </summary>
        private Configuration dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        public Configuration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="window">
        /// The window that will be customized.
        /// </param>
        public Configuration(Window window)
        {
            this.window = window;

            // Listen out for the window resizing
            this.window.StateChanged += (sender, e) =>
            {
                // Fire off events for all properties that are affected by a resize
                this.WindowResized();
            };

            // Create commands
            this.MinimizeCommand = new DelegateCommand(() => this.window.WindowState = WindowState.Minimized);
            this.MaximizeCommand = new DelegateCommand(() => this.window.WindowState ^= WindowState.Maximized);
            this.CloseCommand = new DelegateCommand(() => this.window.Close());
            this.MenuCommand = new DelegateCommand(() => SystemCommands.ShowSystemMenu(this.window, this.GetMousePosition()));

            // Fix window resize issue
            var resizer = new WindowResizer(this.window);

            // Listen out for dock changes
            resizer.WindowDockChanged += (dock) =>
            {
                // Store last position
                this.dockPosition = dock;

                // Fire off resize events
                this.WindowResized();
            };
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the height of the title bar / caption of the window
        /// </summary>
        public GridLength TitleHeightGridLength => new GridLength(this.TitleHeight + this.ResizeBorder);

        /// <summary>
        /// Gets or sets the radius of the edges of the window
        /// </summary>
        public CornerRadius WindowCornerRadius => new CornerRadius(this.WindowRadius);

        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        public Configuration DataContext
        {
            get => this.dataContext;
            set => this.SetProperty(ref this.dataContext, value);
        }

        /// <summary>
        /// Gets or sets the size of the resize border around the window, taking into account the outer margin
        /// </summary>
        public Thickness ResizeBorderThickness
        {
            get => this.resizeBorderThickness; set
            {
                this.resizeBorderThickness = value;
                this.SetProperty(
                    ref this.resizeBorderThickness,
                    new Thickness(this.ResizeBorder + this.OuterMarginSize + 100));
            }
        }

        /// <summary>
        /// Gets or sets the padding of the inner content of the main window
        /// </summary>
        public Thickness InnerContentPadding => new Thickness(this.ResizeBorder);

        /// <summary>
        /// Gets or sets the margin around the window to allow for a drop shadow
        /// </summary>
        public Thickness OuterMarginSizeThickness => new Thickness(this.OuterMarginSize);

        /// <summary>
        /// Gets or sets the smallest width the window can go to
        /// </summary>
        public double WindowMinimumWidth { get; set; } = 400;

        /// <summary>
        /// Gets or sets the smallest height the window can go to
        /// </summary>
        public double WindowMinimumHeight { get; set; } = 400;

        /// <summary>
        /// Gets a value indicating whether the window should be borderless because it is docked or maximized.
        /// </summary>.
        public bool Borderless =>
            (this.window.WindowState == WindowState.Maximized
             || this.dockPosition != WindowDockPosition.Undocked);

        /// <summary>
        /// Gets or sets the size of the resize border around the window
        /// </summary>
        public int ResizeBorder { get; set; } = 6;

        /// <summary>
        /// Gets or sets the margin around the window to allow for a drop shadow
        /// </summary>
        public int OuterMarginSize
        {
            // If it is maximized or docked, no border
            get => this.Borderless ? 0 : this.outerMarginSize;
            set => this.outerMarginSize = value;
        }

        /// <summary>
        /// Gets or sets the radius of the edges of the window
        /// </summary>
        public int WindowRadius
        {
            // If it is maximized or docked, no border
            get => this.Borderless ? 0 : this.windowRadius;
            set => this.windowRadius = value;
        }

        /// <summary>
        /// Gets or sets the height of the title bar / caption of the window
        /// </summary>
        public int TitleHeight { get; set; } = 42;

        #endregion

        #region Commands

        /// <summary>
        /// Gets or sets the command to minimize the window
        /// </summary>
        public ICommand MinimizeCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to maximize the window
        /// </summary>
        public ICommand MaximizeCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to close the window
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to show the system menu of the window
        /// </summary>
        public ICommand MenuCommand { get; set; }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Gets the current mouse position on the screen
        /// </summary>
        /// <returns> Position of cursor relative to window. </returns>
        private Point GetMousePosition()
        {
            // Position of the mouse relative to the window
            var position = Mouse.GetPosition(this.window);

            // Add the window position so its a "ToScreen"
            return new Point(position.X + this.window.Left, position.Y + this.window.Top);
        }

        /// <summary>
        /// If the window resizes to a special position (docked or maximized)
        /// this will update all required property change events to set the borders and radius values
        /// </summary>
        private void WindowResized()
        {
            // Fire off events for all properties that are affected by a resize
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Borderless)));
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.ResizeBorderThickness)));
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.OuterMarginSize)));
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.OuterMarginSizeThickness)));
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.WindowRadius)));
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.WindowCornerRadius)));
        }

        #endregion
    }
}
