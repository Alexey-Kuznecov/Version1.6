
namespace UnityCommander.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;

    using Prism.Commands;
    using Prism.Mvvm;
    using UnityCommander.Behavior;

    /// <summary>
    /// The main window view model.
    /// </summary>
    public partial class MainWindowViewModel : BindableBase
    {
        #region Private Member

        /// <summary>
        /// The window this view model controls
        /// </summary>
        private Window mWindow;

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        private int mOuterMarginSize = 10;

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        private int mWindowRadius = 10;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition mDockPosition = WindowDockPosition.Undocked;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="window">
        /// The window.
        /// </param>
        public MainWindowViewModel(Window window)
        {
            this.mWindow = window;

            // Listen out for the window resizing
            this.mWindow.StateChanged += (sender, e) =>
            {
                // Fire off events for all properties that are affected by a resize
                this.WindowResized();
            };

            // Create commands
            this.MinimizeCommand = new DelegateCommand(() => this.mWindow.WindowState = WindowState.Minimized );
            this.MaximizeCommand = new DelegateCommand(() => this.mWindow.WindowState ^= WindowState.Maximized);
            this.CloseCommand = new DelegateCommand(() => this.mWindow.Close());
            this.MenuCommand = new DelegateCommand(() => SystemCommands.ShowSystemMenu(this.mWindow, this.GetMousePosition()));

            // Fix window resize issue
            var resizer = new WindowResizer(this.mWindow);

            // Listen out for dock changes
            resizer.WindowDockChanged += (dock) =>
            {
                // Store last position
                this.mDockPosition = dock;

                // Fire off resize events
                this.WindowResized();
            };
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the smallest width the window can go to
        /// </summary>
        public double WindowMinimumWidth { get; set; } = 400;

        /// <summary>
        /// Gets or sets the smallest height the window can go to
        /// </summary>
        public double WindowMinimumHeight { get; set; } = 400;

        /// <summary>
        /// True if the window should be borderless because it is docked or maximized
        /// </summary>
        public bool BorderLess => (this.mWindow.WindowState == WindowState.Maximized || this.mDockPosition != WindowDockPosition.Undocked);

        /// <summary>
        /// Gets or sets the size of the resize border around the window
        /// </summary>
        public int ResizeBorder { get; set; } = 6;

        /// <summary>
        /// Gets the size of the resize border around the window, taking into account the outer margin
        /// </summary>
        public Thickness ResizeBorderThickness => new Thickness(this.ResizeBorder + this.OuterMarginSize);

        /// <summary>
        /// Gets the padding of the inner content of the main window
        /// </summary>
        public Thickness InnerContentPadding => new Thickness(this.ResizeBorder);

        /// <summary>
        /// Gets or sets the margin around the window to allow for a drop shadow
        /// </summary>
        public int OuterMarginSize
        {
            // If it is maximized or docked, no border
            get => this.BorderLess ? 0 : this.mOuterMarginSize;
            set => this.mOuterMarginSize = value;
        }

        /// <summary>
        /// Gets the margin around the window to allow for a drop shadow
        /// </summary>
        public Thickness OuterMarginSizeThickness => new Thickness(this.OuterMarginSize);

        /// <summary>
        /// Gets or sets the radius of the edges of the window
        /// </summary>
        public int WindowRadius
        {
            // If it is maximized or docked, no border
            get => this.BorderLess ? 0 : this.mWindowRadius;
            set => this.mWindowRadius = value;
        }

        /// <summary>
        /// Gets the radius of the edges of the window
        /// </summary>
        public CornerRadius WindowCornerRadius => new CornerRadius(this.WindowRadius);

        /// <summary>
        /// Gets or sets the height of the title bar / caption of the window
        /// </summary>
        public int TitleHeight { get; set; } = 42;

        /// <summary>
        /// Gets the height of the title bar / caption of the window
        /// </summary>
        public GridLength TitleHeightGridLength => new GridLength(this.TitleHeight + this.ResizeBorder);

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
        /// <returns></returns>
        private Point GetMousePosition()
        {
            // Position of the mouse relative to the window
            var position = Mouse.GetPosition(this.mWindow);

            // Add the window position so its a "ToScreen"
            return new Point(position.X + this.mWindow.Left, position.Y + this.mWindow.Top);
        }

        /// <summary>
        /// If the window resizes to a special position (docked or maximized)
        /// this will update all required property change events to set the borders and radius values
        /// </summary>
        private void WindowResized()
        {
            // Fire off events for all properties that are affected by a resize
            this.OnPropertyChanged(nameof(this.BorderLess));
            this.OnPropertyChanged(nameof(this.ResizeBorderThickness));
            this.OnPropertyChanged(nameof(this.OuterMarginSize));
            this.OnPropertyChanged(nameof(this.OuterMarginSizeThickness));
            this.OnPropertyChanged(nameof(this.WindowRadius));
            this.OnPropertyChanged(nameof(this.WindowCornerRadius));
        }


        #endregion
    }
}
