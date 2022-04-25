
namespace WindowCustomizer
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;

    using Prism.Commands;
    using Prism.Mvvm;

    /// <summary>
    /// The View Model for the custom flat window
    /// </summary>
    public class CustomViewModel : BindableBase
    {
        #region Private Member

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        private int outerMarginSize = 10;

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        private int windowRadius;

        /// <summary>
        /// The resize border thickness.
        /// </summary>
        private Thickness resizeBorderThickness;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition dockPosition = WindowDockPosition.Undocked;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomViewModel"/> class.
        /// </summary>
        /// <param name="window">
        /// The window that will be customized.
        /// </param>
        public CustomViewModel(Window window)
        {
            this.Window = window;

            // Listen out for the window resizing
            this.Window.StateChanged += (sender, e) =>
            {
                // Fire off events for all properties that are affected by a resize
                this.WindowResized();
            };

            this.CloseCommand = new DelegateCommand(() => { this.Window.Close(); });

            // Fix window resize issue
            var resizer = new WindowResizer(this.Window);

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
        /// Gets or sets the window this view model controls
        /// </summary>
        public Window Window { get; set; }

        /// <summary>
        /// Gets or sets the height of the title bar / caption of the window
        /// </summary>
        public GridLength TitleHeightGridLength => new GridLength(this.TitleHeight + this.ResizeBorder);

        /// <summary>
        /// Gets or sets the radius of the edges of the window
        /// </summary>
        public CornerRadius WindowCornerRadius => new CornerRadius(this.WindowRadius);

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
            (this.Window.WindowState == WindowState.Maximized
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
        public ICommand MinimizeCommand => new DelegateCommand(() => this.Window.WindowState = WindowState.Minimized);

        /// <summary>
        /// Gets or sets the command to maximize the window
        /// </summary>
        public ICommand MaximizeCommand => new DelegateCommand(() => this.Window.WindowState ^= WindowState.Maximized);

        /// <summary>
        /// Gets or sets the command to close the window
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to close the window
        /// </summary>
        public ICommand CollapseRibbonCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to show the system menu of the window
        /// </summary>
        public ICommand MenuCommand => new DelegateCommand(() => SystemCommands.ShowSystemMenu(this.Window, this.GetMousePosition()));

        #endregion

        #region Private Helpers

        /// <summary>
        /// Gets the current mouse position on the screen
        /// </summary>
        /// <returns> Position of cursor relative to window. </returns>
        private Point GetMousePosition()
        {
            // Position of the mouse relative to the window
            var position = Mouse.GetPosition(this.Window);

            // Add the window position so its a "ToScreen"
            return new Point(position.X + this.Window.Left, position.Y + this.Window.Top);
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
