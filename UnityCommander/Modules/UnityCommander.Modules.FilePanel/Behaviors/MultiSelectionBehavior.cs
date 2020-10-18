
namespace UnityCommander.Modules.FilePanel.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System.Windows.Media;

    using UnityCommander.Common.Models;

    /// <summary>
    /// The multi selection directory items behavior.
    /// </summary>
    public class MultiSelectionBehavior : Behavior<ListBox>
    {
        /// <summary>
        /// The selected items property.
        /// </summary>
        private static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(DirectoryBase), typeof(MultiSelectionBehavior), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the selected items.
        /// </summary>
        public DirectoryBase SelectedItems
        {
            get => (DirectoryBase)this.GetValue(SelectedItemsProperty);
            set => this.SetValue(SelectedItemsProperty, value);
        }

        /// <summary>
        /// The on attached.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewMouseDown += this.AssociatedObjectPreviewMouseDown;
        }

        /// <summary>
        /// The on detaching.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.PreviewMouseDown -= this.AssociatedObjectPreviewMouseDown;
        }

        /// <summary>
        /// The associated object preview mouse down.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The e. </param>
        private void AssociatedObjectPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListBox lb = (ListBox)sender;

            if (e.ChangedButton == MouseButton.Right)
            {
                lb.SelectionMode = SelectionMode.Multiple;

                var container = this.GetContainer<ListBoxItem>(lb, e);

                if (container != null)
                {
                    this.SelectedItems = (DirectoryBase)container.Content;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// The select list box item.
        /// </summary>
        /// <typeparam name="T"> Object dependency </typeparam>
        /// <param name="lb"> The container. </param>
        /// <param name="e"> The e.  </param>
        /// <returns> The <see cref="bool"/>. </returns>
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
