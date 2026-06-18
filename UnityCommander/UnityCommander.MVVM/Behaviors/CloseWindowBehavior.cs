
namespace UnityCommander.Core.Behaviors
{
    using System.Windows;
    using Microsoft.Xaml.Behaviors;

    public class CloseWindowBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty CloseTriggerProperty =
            DependencyProperty.Register("CloseTrigger", typeof(bool), typeof(CloseWindowBehavior), new PropertyMetadata(false, OnCloseTriggerChanged));

        public bool CloseTrigger
        {
            get => (bool)GetValue(CloseTriggerProperty);
            set => this.SetValue(CloseTriggerProperty, value);
        }

        private static void OnCloseTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as CloseWindowBehavior;

            behavior?.OnCloseTriggerChanged();
        }

        private void OnCloseTriggerChanged()
        {
            if (this.CloseTrigger)
            {
                this.AssociatedObject?.Close();
            }
        }
    }
}
