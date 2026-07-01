
// ReSharper disable All
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

        public string DirectoryPath
        {
            get => (string)GetValue(DirectoryPathProperty);
            set => this.SetValue(DirectoryPathProperty, value);
        }

        public ICommand NavigateCommand
        {
            get => (ICommand)GetValue(NavigateCommandProperty);
            set => this.SetValue(NavigateCommandProperty, value);
        }

        #endregion

        #region Override methods

        protected override void OnRender(DrawingContext dc)
        {
            // SolidColorBrush mySolidColorBrush = "#FFFFFF".StringFormatToSolidColor();
            // Pen myPen = new Pen("#FFFFFF".StringFormatToSolidColor(), 1);
            // Rect myRect = new Rect(0, 0, 500, 50);
            // dc.DrawRectangle(mySolidColorBrush, myPen, myRect);
        }

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

        private static void OnDirectoryPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int counter = 0;
            NavigationPanel panel = (NavigationPanel)d;
            panel.InternalChildren.Clear();

            if (panel.parsePath == null) return;

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
                    Command = panel.NavigateCommand
                };
                var path = panel.parseParams[counter].Substring(0, panel.parseParams[counter].LastIndexOf('\\'));
                navButton.CommandParameter = Normalize(path);

                var grid = CreateGridNavigationItem(navButton, popButton);
                popButton.CommandParameter = new PopupParameters { CurrentItem = grid, Panel = panel, CurrentPath = panel.parseParams[counter] };

                panel.InternalChildren.Add(grid);
                counter++;
            }
        }

        #endregion

        #region Helper methods

        private static string Normalize(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;

            path = path.Trim();

            // E → E:\
            if (path.Length == 1 && char.IsLetter(path[0]))
                return path + @":\";

            // E: → E:\
            if (path.Length == 2 && path[1] == ':' && char.IsLetter(path[0]))
                return path + @"\";

            // Убираем двойные слеши
            while (path.Contains(@"\\"))
                path = path.Replace(@"\\", @"\");

            return path;
        }

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
                //popupBox.IsOpen = true;
                popupBox.PlacementRectangle = new Rect(location.X, location.Y - 5, 0, 0);
                popupBox.Placement = PlacementMode.Top;
                popupBox.StaysOpen = false;

                Binding bind = new Binding("PopupIsOpen") { Mode = BindingMode.TwoWay, Source = popupViewModel };
                BindingOperations.SetBinding(popupBox, Popup.IsOpenProperty, bind);
            }
        }

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

        private static void SetBindingPopButton(Button popButton, PopupViewModel popupViewModel)
        {
            Binding bind = new Binding("PopButtonIsEnabled") { Mode = BindingMode.TwoWay, Source = popupViewModel };
            BindingOperations.SetBinding(popButton, Button.IsEnabledProperty, bind);
        }

        #endregion

        #region Event handlers

        private void NavigationPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            NavigationPanel panel = (NavigationPanel)sender;
            OnDirectoryPathChanged(panel, new DependencyPropertyChangedEventArgs());
            CoerceDirectoryPath(panel, panel.currentPath);
        }

        #endregion

        internal class PopupParameters
        {
            public Grid CurrentItem { get; set; }

            public NavigationPanel Panel { get; set; }

            public string CurrentPath { get; set; }

            public string SelectedPath { get; set; }
        }

        internal class PopupViewModel : BindableBase
        {
            private readonly NavigationPanel currentPanel;

            private bool popButtonIsEnabled;

            private bool popupIsOpen;

            private ObservableCollection<PopupParameters> directoryList;

            public PopupViewModel(PopupParameters parameters)
            {
                this.popupIsOpen = true;
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

            public ObservableCollection<PopupParameters> DirectoryList
            {
                get => this.directoryList;
                set
                {
                    this.directoryList = value;
                    this.SetProperty(ref this.directoryList, value);
                }
            }

            public bool PopupIsOpen
            {
                get => this.popupIsOpen;
                set
                {
                    this.SetProperty(ref this.popupIsOpen, value);
                    this.PopButtonIsEnabled = !this.popupIsOpen;
                } 
            }

            public PopupParameters SelectItem
            {
                set => this.currentPanel.NavigateCommand.Execute(value.SelectedPath);
            }

            public bool PopButtonIsEnabled
            {
                get => this.popButtonIsEnabled;
                set => this.SetProperty(ref this.popButtonIsEnabled, value);
            }
        }
    }
}
