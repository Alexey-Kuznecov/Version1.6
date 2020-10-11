
namespace UnityCommander.Modules.FilePanel.Controls
{
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
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
                margin += child.DesiredSize.Width + 2;

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

            foreach (var gridChild in panel.InternalChildren)
            {
                var grid = (Grid)gridChild;
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
            var panel = (NavigationPanel)d;
            var cmdCounter = 0;
            string[] slp = panel.DirectoryPath.Split('\\');
            string[] cmd = HelperMethods.ParsePath(panel.DirectoryPath);
            panel.InternalChildren.Clear();

            foreach (string item in slp)
            {
                if (item == string.Empty) break;

                PopupParameters parameter = new PopupParameters();
                var navigationPopup = new Button
                {
                    Margin = new Thickness(2),
                    Height = 25,
                    Width = 30,
                    Content = "+",
                    Command = new DelegateCommand<PopupParameters>(CreatePopupNavigation)
                };
                var navigation = new Button
                {
                    Height = 25,
                    Content = item,
                    Command = panel.NavigateCommand,
                    CommandParameter = cmd[cmdCounter]
                };

                var grid = CreateGridNavigationItem(navigation, navigationPopup);
                parameter.Element = grid;
                parameter.Command = panel.NavigateCommand;
                parameter.Path = cmd[cmdCounter];
                navigationPopup.CommandParameter = parameter;
                panel.InternalChildren.Add(grid);
                cmdCounter++;
            }
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Creates a pop-up navigation for each item in the navigation bar.
        /// </summary>
        /// <param name="parameters"> The parameter for pop-up navigation. </param>
        private static void CreatePopupNavigation(PopupParameters parameters)
        {
            // Popup content.
            NavigationPopup content = new NavigationPopup();
            PopupModel navigationPopup = new PopupModel(parameters.Path, parameters.Command);
            content.DataContext = navigationPopup;

            // Popup creation.
            Popup popupBox = new Popup();
            Point location = parameters.Element.PointToScreen(new Point(0, 0));
            popupBox.Child = content;
            popupBox.IsOpen = true;
            popupBox.PlacementRectangle = new Rect(location.X, location.Y, 0, 0);
            popupBox.Placement = PlacementMode.Top;
            popupBox.StaysOpen = false;
        }

        /// <summary>
        /// Creates a grid of the navigation item.
        /// </summary>
        /// <param name="navButton"> The navigation button. </param>
        /// <param name="popButton"> The button pop-up invoker. </param>
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

            return grid;
        }

        #endregion
    }

    /// <summary>
    /// Serves to transferred parameters from the file panel to the pop-up navigation.
    /// </summary>
    [SuppressMessage(
        "StyleCop.CSharp.MaintainabilityRules", 
        "SA1402:FileMayOnlyContainASingleClass", 
        Justification = "Reviewed. Suppression is OK here.")]
    internal class PopupParameters
    {
        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        public UIElement Element { get; set; }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// Gets or sets the command parameters.
        /// </summary>
        public string CommandParams { get; set; }
    }

    /// <summary>
    /// Provides some data to the user control view.
    /// </summary>
    internal class PopupModel : BindableBase
    {    
        /// <summary>
        /// The set command.
        /// </summary>
        private readonly ICommand setCommand;

        /// <summary>
        /// The directory list.
        /// </summary>
        private ObservableCollection<PopupParameters> directoryList;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PopupModel"/> class.
        /// </summary>
        /// <param name="currentPath">
        /// The current path.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        public PopupModel(string currentPath, ICommand command)
        {
            this.DirectoryList = new ObservableCollection<PopupParameters>();

            if (!string.IsNullOrEmpty(currentPath))
            {
                foreach (var item in Directory.GetDirectories(currentPath))
                {
                    PopupParameters model = new PopupParameters { Path = item, CommandParams = item };
                    this.DirectoryList.Add(model);
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
            set => this.SetProperty(ref this.directoryList, value);
        }

        /// <summary>
        /// Sets the select path.
        /// </summary>
        public PopupParameters SelectItem
        {
            set => this.setCommand.Execute(value.Path);
        }
    }
}
