
namespace UnityCommander.Controls.TabPanel
{
    using System;

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
        /// <param name="collection">
        /// The collection.
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
