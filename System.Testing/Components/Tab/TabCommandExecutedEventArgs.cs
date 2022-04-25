
using System.Windows;

namespace Components.Tab
{
    /// <summary>
    /// The value changed event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public delegate void TabCommandExecutedEventHandler(object sender, TabCommandExecutedEventArg e);

    /// <summary>
    /// The tab command executed event arguments.
    /// </summary>
    public class TabCommandExecutedEventArg : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TabCommandExecutedEventArg"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        public TabCommandExecutedEventArg(RoutedEvent id, object command)
        {
            this.Content = command;
            this.RoutedEvent = id;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Content { get; }
    }
}
