
namespace UnityCommander.Modules.FilePanel.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System.Windows.Media;

    using UnityCommander.Common.Models.Directory;

    public class MultiSelectionBehavior : Behavior<ListBox>
    {
        private static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(BaseDirectory), typeof(MultiSelectionBehavior), new UIPropertyMetadata(null));
        public BaseDirectory SelectedItems
        {
            get => (BaseDirectory)this.GetValue(SelectedItemsProperty);
            set => this.SetValue(SelectedItemsProperty, value);
        }
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewMouseDown += this.AssociatedObjectPreviewMouseDown;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.PreviewMouseDown -= this.AssociatedObjectPreviewMouseDown;
        }
        private void AssociatedObjectPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListBox lb = (ListBox)sender;

            if (e.ChangedButton == MouseButton.Right)
            {
                lb.SelectionMode = SelectionMode.Multiple;

                var container = this.GetContainer<ListBoxItem>(lb, e);

                if (container != null)
                {
                    this.SelectedItems = (BaseDirectory)container.Content;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }
        private T GetContainer<T>(Control lb, MouseEventArgs e) where T : DependencyObject
        {
            var point = e.GetPosition(lb);
            var o = VisualTreeHelper.HitTest(lb, point).VisualHit;

            while (o != lb)
            {
                if (o is T checkContainer)
                {
                    return checkContainer;
                }

                o = VisualTreeHelper.GetParent(o ?? throw new InvalidOperationException());
            }

            return null;
        }
    }
}
