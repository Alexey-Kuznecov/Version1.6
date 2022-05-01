
namespace UnityCommander.Controls.TabPanel
{
    using System.Windows;

    /// <summary>
    /// The value changed event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public delegate void ContentChangedEventHandler(object sender, ContentChangedEventArgs e);

    /// <summary>
    /// The value changed event args.
    /// </summary>
    public class ContentChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentChangedEventArgs"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="newContent">
        /// The number.
        /// </param>
        public ContentChangedEventArgs(RoutedEvent id, object newContent)
        {
            this.Content = newContent;
            this.RoutedEvent = id;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Content { get; }
    }
}
