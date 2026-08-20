
using Prism.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnityCommander.Controls.Navigation
{
    public class NavigationPanel : Panel
    {
        private static readonly DependencyProperty DirectoryPathProperty;

        private static readonly DependencyProperty NavigateCommandProperty;

        private static double margin;

        private string currentPath;

        private readonly NavigationPathParser _parser = new NavigationPathParser();
        
        private readonly NavigationItemBuilder _itemBuilder = new NavigationItemBuilder();
        
        private readonly NavigationPopupService _popupService = new NavigationPopupService();

        private NavigationPath? _navigationPath;

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

        public NavigationPanel()
            : base()
        {
            this.SizeChanged += NavigationPanel_SizeChanged;
        }

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

        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);

            foreach (UIElement child in InternalChildren)
            {
                child.Measure(size);
            }

            return new Size();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            margin = 0;

            for (var index = 0; index < InternalChildren.Count; index++)
            {
                UIElement child = InternalChildren[index];
                child.Arrange(new Rect(new Point(margin, 10), child.DesiredSize));
                margin += child.DesiredSize.Width;

                if (margin - 10 > finalSize.Width)
                {
                    if (InternalChildren.Count != 1)
                    {
                        InternalChildren.RemoveAt(1);
                    }
                }
            }

            return finalSize;
        }

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

        private static object CoerceDirectoryPath(
            DependencyObject d,
            object baseValue)
        {
            var panel = (NavigationPanel)d;

            if (baseValue is string path)
            {
                panel.currentPath = path;
                panel._navigationPath =
                    panel._parser.Parse(path);
            }

            return baseValue;
        }

        private static void OnDirectoryPathChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var panel = (NavigationPanel)d;

            panel.Rebuild();
        }

        private void Rebuild()
        {
            InternalChildren.Clear();

            if (_navigationPath == null)
                return;

            foreach (var item in _navigationPath.Items)
            {
                var parameters = new PopupParameters
                {
                    CurrentItem = item,
                    NavigateCommand = NavigateCommand
                };

                var popupCommand =
                    new DelegateCommand<PopupParameters>(
                        SetPopupNavigation);

                var control =
                    _itemBuilder.Build(
                        parameters,
                        popupCommand,
                        NavigateCommand);

                InternalChildren.Add(control);
            }
        }

        private void SetPopupNavigation(PopupParameters parameter)
        {
            _popupService.Show(
                parameter.Anchor,
                parameter.CurrentItem.Path,
                parameter.NavigateCommand);
        }

        private void NavigationPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            NavigationPanel panel = (NavigationPanel)sender;
            OnDirectoryPathChanged(panel, new DependencyPropertyChangedEventArgs());
            CoerceDirectoryPath(panel, panel.currentPath);
        }
    }
}
