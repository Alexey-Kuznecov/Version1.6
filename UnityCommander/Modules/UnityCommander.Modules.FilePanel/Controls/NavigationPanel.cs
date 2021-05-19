
namespace UnityCommander.Modules.FilePanel.Controls
{
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;

    using Prism.Commands;
    using Prism.Mvvm;
    using UnityCommander.Common;
    using UnityCommander.Core.Helper;

    /// <summary>
    /// The navigation panel.
    /// </summary>
    public class NavigationPanel : Panel
    {
        #region Declaration fields

        /// <summary>
        /// The dependency property that is used to synchronize the directories
        /// between file pane and navigation panel.
        /// </summary>
        private static readonly DependencyProperty DirectoryPathProperty;

        /// <summary>
        /// The dependency property that will be bound to each controls
        /// in the navigation bar.
        /// </summary>
        private static readonly DependencyProperty NavigateCommandProperty;

        /// <summary>
        /// The space between navigation controls.
        /// </summary>
        private static double margin;

        /// <summary>
        /// The current path to active bar.
        /// </summary>
        private string currentPath;

        /// <summary>
        /// Contains the paths of the parent directories of the current directory to the root directory.
        /// </summary>
        private string[] parseParams;

        /// <summary>
        /// Contains the names of directories to display as button content.
        /// </summary>
        private string[] parsePath;

        #endregion

        #region Declaration constuctors

        /// <summary>
        /// Initializes static members of the <see cref="NavigationPanel"/> class.
        /// Registers a dependency property.
        /// </summary>
        static NavigationPanel()
        {
            DirectoryPathProperty = DependencyProperty.Register(
                "DirectoryPath",
                typeof(string),
                typeof(NavigationPanel),
                new FrameworkPropertyMetadata(
                    "C:\\",
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange,
                    OnDirectoryPathChanged,
                    CoerceDirectoryPath));

            NavigateCommandProperty = DependencyProperty.Register(
                "NavigateCommand",
                typeof(ICommand),
                typeof(NavigationPanel),
                new FrameworkPropertyMetadata(
                    new DelegateCommand(() => { }),
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                    OnNavigateCommandChanged));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationPanel"/> class.
        /// </summary>
        public NavigationPanel()
            : base()
        {
            this.SizeChanged += this.NavigationPanel_SizeChanged;
        }

        #endregion

        #region Dependency properties

        /// <summary>
        /// Gets or sets the path to directory that is used to synchronize
        /// the directories between file pane and navigation pane.
        /// </summary>
        public string DirectoryPath
        {
            get => (string)GetValue(DirectoryPathProperty);
            set => this.SetValue(DirectoryPathProperty, value);
        }

        /// <summary>
        /// Gets or sets the command that will be bound to each control in the navigation bar.
        /// </summary>
        public ICommand NavigateCommand
        {
            get => (ICommand)GetValue(NavigateCommandProperty);
            set => this.SetValue(NavigateCommandProperty, value);
        }

        #endregion

        #region Override methods

        /// <summary>
        /// The on render.
        /// </summary>
        /// <param name="dc">
        /// The dc.
        /// </param>
        protected override void OnRender(DrawingContext dc)
        {
            // SolidColorBrush mySolidColorBrush = "#FFFFFF".StringFormatToSolidColor();
            // Pen myPen = new Pen("#FFFFFF".StringFormatToSolidColor(), 1);
            // Rect myRect = new Rect(0, 0, 500, 50);
            // dc.DrawRectangle(mySolidColorBrush, myPen, myRect);
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required
        /// for child elements and determines a size for the FrameworkElement-derived class.
        /// </summary>
        /// <param name="availableSize">
        /// The available size that this element can give to child elements.
        /// Infinity can be specified as a value to indicate that the element will
        /// size to whatever content is available.
        /// </param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);

            // In our example, we just have one child. 
            // Report that our panel requires just the size of its only child.
            foreach (UIElement child in this.InternalChildren)
            {
                child.Measure(size);
            }

            return new Size();
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines
        /// a size for a FrameworkElement derived class.
        /// </summary>
        /// <param name="finalSize">
        /// The final area within the parent that this
        /// element should use to arrange itself and its children.
        /// </param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            margin = 0;

            for (var index = 0; index < this.InternalChildren.Count; index++)
            {
                UIElement child = this.InternalChildren[index];
                child.Arrange(new Rect(new Point(margin, 10), child.DesiredSize));
                margin += child.DesiredSize.Width;

                if (margin - 10 > finalSize.Width)
                {
                    if (InternalChildren.Count != 1)
                    {
                        this.InternalChildren.RemoveAt(1);
                    }
                }
            }

            return finalSize;
        }

        #endregion

        #region Declaration callback functions

        /// <summary>
        /// Represents the callback that is invoked when the effective
        /// property value of a dependency property changed.
        /// </summary>
        /// <param name="d"> The DependencyObject on which the property has changed value. </param>
        /// <param name="e"> Event data that is issued by any event that tracks changes to the effective value of this property. </param>
        private static void OnNavigateCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (NavigationPanel)d;
            var command = (DelegateCommand<object>)e.NewValue;

            if (command == null) return;

            foreach (var borderChild in panel.InternalChildren)
            {
                var grid = (Grid)borderChild;
                var navButton = (Button)grid.Children[0];
                navButton.Command = command;
            }
        }

        /// <summary>
        /// Provides a template for the method that is called whenever a dependency property
        /// value is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="d">
        /// The <see langword="object"/> that the property exists on.
        /// When the callback is invoked, the property system will pass this value.
        /// </param>
        /// <param name="baseValue"> The new value of the property, prior to any coercion attempt. </param>
        /// <returns> The coerced value (with appropriate type). </returns>
        private static object CoerceDirectoryPath(DependencyObject d, object baseValue)
        {
            NavigationPanel panel = (NavigationPanel)d;

            if (baseValue != null)
            {
                panel.currentPath = (string)baseValue;
                panel.parseParams = HelperFunctions.ParsePath(panel.currentPath);
                panel.parsePath = panel.currentPath.Split('\\');
            }

            return baseValue;
        }

        /// <summary>
        /// Represents the callback that is invoked when the effective
        /// property value of a dependency property changed.
        /// </summary>
        /// <param name="d"> The DependencyObject on which the property has changed value. </param>
        /// <param name="e"> Event data that is issued by any event that tracks changes to the effective value of this property. </param>
        private static void OnDirectoryPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int counter = 0;
            NavigationPanel panel = (NavigationPanel)d;
            panel.InternalChildren.Clear();

            while (counter < panel.parsePath.Length)
            {
                if (panel.parsePath[counter] == string.Empty) break;

                var popButton = new Button
                {
                    Style = (Style)Application.Current.FindResource("NavigationPopupButtonStyle"),
                    Command = new DelegateCommand<PopupParameters>(SetPopupNavigation)
                };
                var navButton = new Button
                {
                    Style = (Style)Application.Current.FindResource("NavigationBackButtonStyle"),
                    Content = panel.parsePath[counter],
                    Command = panel.NavigateCommand,
                    CommandParameter = panel.parseParams[counter]
                };

                var grid = CreateGridNavigationItem(navButton, popButton);
                popButton.CommandParameter = new PopupParameters { CurrentItem = grid, Panel = panel, CurrentPath = panel.parseParams[counter] };

                panel.InternalChildren.Add(grid);
                counter++;
            }
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Creates a pop-up menu for each item in the navigation bar.
        /// </summary>
        /// <param name="parameters"> The external arguments for pop-up menu. </param>
        private static void SetPopupNavigation(PopupParameters parameters)
        {
            Grid navItem = parameters.CurrentItem;
            Button popButton = navItem?.Children[1] as Button;

            // Popup content.
            NavigationPopup popupControl = new NavigationPopup();
            PopupViewModel popupViewModel = new PopupViewModel(parameters);
            popupControl.DataContext = popupViewModel;
            SetBindingPopButton(popButton, popupViewModel);

            // Popup creation.
            if (navItem != null)
            {
                Popup popupBox = new Popup();
                Point location = navItem.PointToScreen(new Point(0, 0));
                popupBox.Child = popupControl;
                popupBox.IsOpen = true;
                popupBox.PlacementRectangle = new Rect(location.X, location.Y - 5, 0, 0);
                popupBox.Placement = PlacementMode.Top;
                popupBox.StaysOpen = false;
            }
        }

        /// <summary>
        /// Creates a grid of the navigation button.
        /// </summary>
        /// <param name="navButton"> The navigation button. </param>
        /// <param name="popButton"> The pop-up menu button. </param>
        /// <returns>
        /// Returns the button wrapped in grid.
        /// </returns>
        private static Grid CreateGridNavigationItem(Button navButton, Button popButton)
        {
            Grid grid = new Grid();
            ColumnDefinition gridColumn = new ColumnDefinition();
            ColumnDefinition gridColumn2 = new ColumnDefinition();
            grid.ColumnDefinitions.Add(gridColumn);
            grid.ColumnDefinitions.Add(gridColumn2);
            Grid.SetColumn(navButton, 0);
            Grid.SetColumn(popButton, 1);
            grid.Children.Add(navButton);
            grid.Children.Add(popButton);
            grid.Style = (Style)Application.Current.FindResource("NavigationButtonShadowStyle");

            return grid;
        }

        /// <summary>
        /// Binds two properties, <see cref="PopupViewModel.PopButtonIsEnabled"/> and <see cref="Button.IsEnabledProperty"/>.
        /// </summary>
        /// <param name="popButton"> The pop-up menu button. </param>
        /// <param name="popupViewModel"> The pop-up menu view model. </param>
        private static void SetBindingPopButton(Button popButton, PopupViewModel popupViewModel)
        {
            Binding bind = new Binding("PopButtonIsEnabled") { Mode = BindingMode.TwoWay, Source = popupViewModel };
            BindingOperations.SetBinding(popButton, Button.IsEnabledProperty, bind);
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// When the navigation bar is resized, this handler updates it.
        /// </summary>
        /// <param name="sender"> The current navigation bar. </param>
        /// <param name="e"> Resize information.. </param>
        private void NavigationPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            NavigationPanel panel = (NavigationPanel)sender;
            OnDirectoryPathChanged(panel, new DependencyPropertyChangedEventArgs());
            CoerceDirectoryPath(panel, panel.currentPath);
        }

        #endregion

        /// <summary>
        /// Serves to transferred parameters from the navigation bar to the pop-up menu.
        /// </summary>
        internal class PopupParameters
        {
            /// <summary>
            /// Gets or sets current item in the navigation panel.
            /// </summary>
            public Grid CurrentItem { get; set; }

            /// <summary>
            /// Gets or sets the link to the navigation panel for the current file panel.
            /// </summary>
            public NavigationPanel Panel { get; set; }

            /// <summary>
            /// Gets or sets the path to the directory where the pop-up menu will be invoked.
            /// </summary>
            public string CurrentPath { get; set; }

            /// <summary>
            /// Gets or sets the selected directory path in the pop-up menu.
            /// </summary>
            public string SelectedPath { get; set; }
        }

        /// <summary>
        /// Provides some data to the user control view.
        /// </summary>
        internal class PopupViewModel : BindableBase
        {
            /// <summary>
            /// The reference to the current navigation bar.
            /// </summary>
            private readonly NavigationPanel currentPanel;

            /// <summary>
            /// The state of the pop-up menu button.
            /// </summary>
            private bool popButtonIsEnabled;

            /// <summary>
            /// The list directories.
            /// </summary>
            private ObservableCollection<PopupParameters> directoryList;

            /// <summary>
            /// Initializes a new instance of the <see cref="PopupViewModel"/> class.
            /// </summary>
            /// <param name="parameters"> The external arguments for pop-up menu. </param>
            public PopupViewModel(PopupParameters parameters)
            {
                this.currentPanel = parameters.Panel;

                this.DirectoryList = new ObservableCollection<PopupParameters>();

                DirectoryInfo dir = new DirectoryInfo(parameters.CurrentPath);

                foreach (var item in dir.GetDirectories())
                {
                    if ((item.Attributes & FileAttributes.Hidden) == 0)
                    {
                        PopupParameters model = new PopupParameters { SelectedPath = item.FullName };
                        this.DirectoryList.Add(model);
                    }
                }
            }

            /// <summary>
            /// Gets or sets a list of the directories.
            /// </summary>
            public ObservableCollection<PopupParameters> DirectoryList
            {
                get => this.directoryList;
                set
                {
                    this.directoryList = value;
                    this.SetProperty(ref this.directoryList, value);
                }
            }

            /// <summary>
            /// Sets the selected directory path.
            /// </summary>
            public PopupParameters SelectItem
            {
                set
                {
                    this.currentPanel.NavigateCommand.Execute(value.SelectedPath);
                    this.PopButtonIsEnabled = false;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether pop-up menu button is enabled.
            /// </summary>
            public bool PopButtonIsEnabled
            {
                get => this.popButtonIsEnabled;
                set => this.SetProperty(ref this.popButtonIsEnabled, value);
            }
        }
    }
}
