
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace UnityCommander.Modules.FilePanel.Behaviors
{
    public class ContextMenuBehavior : Behavior<ListView>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(ContextMenuBehavior),
                new PropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseRightButtonDown += OnRightClick;
        }

        private void OnRightClick(object sender, MouseButtonEventArgs e)
        {
            var element = e.OriginalSource as DependencyObject;

            var item = ItemsControl.ContainerFromElement(
                AssociatedObject, element) as ListViewItem;

            if (item != null)
            {
                item.IsSelected = true;
                Command?.Execute(item.DataContext);
            }
            else
            {
                Command?.Execute(null);
            }
        }
    }
}
