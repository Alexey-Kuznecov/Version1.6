
namespace UnityCommander.Modules.FilePanel.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;

    using Prism.Commands;
    using Prism.Mvvm;
    using UnityCommander.Common;

    /// <summary>
    /// The navigation panel.
    /// </summary>
    public class NavigationPanel : Panel
    {
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
        /// The pop-up model class instance.
        /// </summary>
        private static PopupViewModel popupViewModel;

        /// <summary>
        /// The pop-up model class instance.
        /// </summary>
        private static NavigationPopup popupControl;

        /// <summary>
        /// The space between navigation controls.
        /// </summary>
        private static double margin;

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
        
        public List<FrameworkElement> VisualElementList { get; private set; }

        #endregion

        #region Override methods

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

            for (var index = 0; index < InternalChildren.Count; index++)
            {
                UIElement child = this.InternalChildren[index];
                child.Arrange(new Rect(new Point(margin, 10), child.DesiredSize));
                margin += child.DesiredSize.Width + 5;

                if (margin - 10 > finalSize.Width)
                {
                    this.InternalChildren.RemoveAt(1);
                }
            }

            // Returns the final Arranged size
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

            foreach (var borderChild in panel.InternalChildren)
            {
                var grid = (Grid)borderChild;
                var popButton = (Button)grid.Children[1];

                ((Button)grid.Children[0]).Command = command;
                PopupParameters parameters = (PopupParameters)popButton.CommandParameter;
                parameters.Command = command;
                popButton.CommandParameter = parameters;
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
            int cmdCounter = 0;
            NavigationPanel panel = (NavigationPanel)d;

            /*
             * Parse the path into directory names to display the contents of the buttons
             * in the navigation panel and create parameters by expanding the path to the full
             * paths of subdirectories.
             */
            string[] slp = panel.DirectoryPath.Split('\\');
            string[] cmd = HelperMethods.ParsePath(panel.DirectoryPath);
          
            panel.InternalChildren.Clear();

            foreach (string item in slp)
            {
                if (item == string.Empty) break;

                PopupParameters parameter = new PopupParameters();
                Grid grid = SetGridFromXaml();
                if (grid != null)
                {
                    var navButton = (Button)grid.Children[0];
                    var popButton = (Button)grid.Children[1];

                    popButton.Height = 25;
                    popButton.Width = 30;
                    popButton.Command = new DelegateCommand<PopupParameters>(SetPopupNavigation);

                    navButton.Content = item;
                    navButton.Command = panel.NavigateCommand;
                    navButton.CommandParameter = cmd[cmdCounter];

                    parameter.GridElement = grid;
                    parameter.PopButton = popButton;
                    parameter.Command = panel.NavigateCommand;
                    parameter.Path = cmd[cmdCounter];
                    popButton.CommandParameter = parameter;
                    panel.InternalChildren.Add(grid);
                    cmdCounter++;
                }
            }
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Creates a grid of the navigation item.
        /// </summary>
        /// <returns> Returns the button wrapped in grid. </returns>
        private static Grid SetGridFromXaml()
        {
            Grid grid = new Grid();

            ResourceDictionary resourceDictionary = new ResourceDictionary
            {
                Source = new Uri(@"pack://application:,,,/UnityCommander.Modules.FilePanel;component/Resources/NavigationBarStyles.xaml")
            };

            foreach (DictionaryEntry entry in resourceDictionary)
            {
                if (entry.Key.ToString() == "PopupButton")
                {
                    grid = (Grid)entry.Value;
                    return grid;
                }
            }

            return grid;
        }

        /// <summary>
        /// Creates a pop-up navigation for each item in the navigation bar.
        /// </summary>
        /// <param name="parameters"> The parameters for pop-up navigation. </param>
        private static void SetPopupNavigation(PopupParameters parameters)
        {
            // Popup content.
            popupControl = new NavigationPopup();
            popupViewModel = new PopupViewModel(parameters.Path, parameters.Command);
            popupControl.DataContext = popupViewModel;
            SetBindingPopButton(parameters);
            
            // Popup creation.
            Popup popupBox = new Popup();
            Point location = parameters.GridElement.PointToScreen(new Point(0, 0));
            popupBox.Child = popupControl;
            popupBox.IsOpen = true;
            popupBox.PlacementRectangle = new Rect(location.X, location.Y - 5, 0, 0);
            popupBox.Placement = PlacementMode.Top;
            popupBox.StaysOpen = false;
        }

        /// <summary>
        /// Binds the enable property of the button to the view model property of the popup window.
        /// </summary>
        /// <param name="parameters"> The parameters for pop-up navigation. </param>
        private static void SetBindingPopButton(PopupParameters parameters)
        {
            Button popBt = parameters.PopButton;
            Binding bind = new Binding("PopButtonIsEnabled") { Mode = BindingMode.TwoWay, Source = popupViewModel };
            BindingOperations.SetBinding(popBt, Button.IsEnabledProperty, bind);
        }

        #endregion

        /// <summary>
        /// The navigation panel size changed.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The e. </param>
        private void NavigationPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var panel = (NavigationPanel)sender;
        }

        /// <summary>
        /// Serves to transferred parameters from the file panel to the pop-up navigation.
        /// </summary>
        internal class PopupParameters
        {
            /// <summary>
            /// Gets or sets the path.
            /// </summary>
            public string Path { get; set; }

            /// <summary>
            /// Gets or sets the element.
            /// </summary>
            public Grid GridElement { get; set; }

            /// <summary>
            /// Gets or sets the pop button.
            /// </summary>
            public Button PopButton { get; set; }

            /// <summary>
            /// Gets or sets the command.
            /// </summary>
            public ICommand Command { get; set; }

            /// <summary>
            /// Gets or sets the command parameters.
            /// </summary>
            public string CommandParams { get; set; }

            /// <summary>
            /// Gets or sets the command parameters.
            /// </summary>
            public Binding Bound { get; set; }

            /// <summary>
            /// Gets or sets the command parameters.
            /// </summary>
            public Binding NavButton { get; set; }
        }

        /// <summary>
        /// Provides some data to the user control view.
        /// </summary>
        internal class PopupViewModel : BindableBase
        {
            /// <summary>
            /// The set command.
            /// </summary>
            private readonly ICommand setCommand;

            /// <summary>
            /// The set command.
            /// </summary>
            private bool popButtonIsEnabled;

            /// <summary>
            /// The directory list.
            /// </summary>
            private ObservableCollection<PopupParameters> directoryList;

            /// <summary>
            /// Initializes a new instance of the <see cref="PopupViewModel"/> class.
            /// </summary>
            /// <param name="currentPath">
            /// The current path.
            /// </param>
            /// <param name="command">
            /// The command.
            /// </param>
            public PopupViewModel(string currentPath, ICommand command)
            {
                this.DirectoryList = new ObservableCollection<PopupParameters>();

                if (!string.IsNullOrEmpty(currentPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(currentPath);

                    foreach (var item in dir.GetDirectories())
                    {
                        if ((item.Attributes & FileAttributes.Hidden) == 0)
                        {
                            PopupParameters model = new PopupParameters { Path = item.FullName, CommandParams = item.FullName };
                            this.DirectoryList.Add(model);
                        }
                    }
                }

                this.setCommand = command;
            }

            /// <summary>
            /// Gets or sets a list of directory path.
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
            /// Sets the select path.
            /// </summary>
            public PopupParameters SelectItem
            {
                set
                {
                    this.setCommand.Execute(value.Path);
                    this.PopButtonIsEnabled = false;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether pop-up button is enabled.
            /// </summary>
            public bool PopButtonIsEnabled
            {
                get => this.popButtonIsEnabled;
                set => this.SetProperty(ref this.popButtonIsEnabled, value);
            }
        }
    }
}
