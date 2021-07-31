
namespace UnityCommander.Controls.Taber
{
    using System;
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
    public delegate void CollectionChangedEventHandler(object sender, CollectionChangedEventArg e);

    /// <summary>
    /// The tab command executed event arguments.
    /// </summary>
    public class CollectionChangedEventArg : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionChangedEventArg"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        public CollectionChangedEventArg(TabCollection collection)
        {
            this.Collection = collection;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public TabCollection Collection { get; }
    }
}
