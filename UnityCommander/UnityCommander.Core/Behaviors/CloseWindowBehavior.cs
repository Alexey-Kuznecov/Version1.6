
namespace UnityCommander.Core.Behaviors
{
    using System.Windows;
    using System.Windows.Interactivity;

    /// <summary>
    /// The close window behavior.
    /// </summary>
    public class CloseWindowBehavior : Behavior<Window>
    {
        /// <summary>
        /// The close trigger property.
        /// </summary>
        public static readonly DependencyProperty CloseTriggerProperty =
            DependencyProperty.Register("CloseTrigger", typeof(bool), typeof(CloseWindowBehavior), new PropertyMetadata(false, OnCloseTriggerChanged));

        /// <summary>
        /// Gets or sets a value indicating whether close trigger.
        /// </summary>
        public bool CloseTrigger
        {
            get => (bool)GetValue(CloseTriggerProperty);
            set => this.SetValue(CloseTriggerProperty, value);
        }

        /// <summary>
        /// The on close trigger changed.
        /// </summary>
        /// <param name="d"> The d. </param>
        /// <param name="e"> The e. </param>
        private static void OnCloseTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as CloseWindowBehavior;

            behavior?.OnCloseTriggerChanged();
        }

        /// <summary>
        /// The on close trigger changed.
        /// </summary>
        private void OnCloseTriggerChanged()
        {
            // When close trigger is true, close the window
            if (this.CloseTrigger)
            {
                this.AssociatedObject?.Close();
            }
        }
    }
}
