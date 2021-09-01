
namespace UnityCommander.Controls.Taber
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;

    /// <summary>
    /// The tab collection.
    /// </summary>
    [SuppressMessage("ReSharper", "StyleCop.SA1503")]
    public class TabCollection : CollectionBase
    {
        /// <summary>
        /// The collection changed.
        /// </summary>
        public event EventHandler CollectionChanged;

        #region Properties
        
        /// <summary>
        /// Gets/Sets value for the item by that index
        /// </summary>
        public TabControl this[int index]
        {
            get => (TabControl)this.List[index];
            set => this.List[index] = value;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The index of.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int IndexOf(TabControl control)
        {
            if (control != null)
            {
                return base.List.IndexOf(control);
            }

            return -1;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Add(TabControl control)
        {
            if (control != null)
            {
                control.TabClick += this.OnControlTabClick;
                var position = this.List.Add(control);
                this.RaiseEvent(this);
                return position;
            }
            return -1;
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        public void Remove(TabControl control)
        {
            control.TabClick -= this.OnControlTabClick;
            this.InnerList.Remove(control);
            this.RaiseEvent(this);
        }

        /// <summary>
        /// The add range.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public void AddRange(TabCollection collection)
        {
            if (collection != null)
            {
                this.InnerList.AddRange(collection);
            }
        }

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="control">
        /// The control.
        /// </param>
        public void Insert(int index, TabControl control)
        {
            if (index <= List.Count && control != null)
            {
                this.List.Insert(index, control);
                this.RaiseEvent(this);
            }
        }

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Contains(TabControl control)
        {
            return this.List.Contains(control);
        }

        #endregion

        /// <summary>
        /// The on collection changed event.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void OnCollectionChangedEvent(EventArgs e)
        {
            EventHandler handler = this.CollectionChanged;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// The raise event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        private void RaiseEvent(object sender)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged.Invoke(this, new CollectionChangedEventArg((TabCollection)this.List));
            }
        }

        /// <summary>
        /// The control_ tab click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnControlTabClick(object sender, RoutedEventArgs e)
        {
            var control = (TabControl)sender;
            var panel = (e as TabCommandExecutedEventArg)?.Content as TabPanel;

            if ((string)control.Content == "+")
                return;

            foreach (TabControl item in this.List)
            {
                if ((string)item.Content == "+")
                    continue;
                if (item.Equals(control))
                    item.IsEnabled = false;
                else
                    item.IsEnabled = true;
            }
        }
    }
}
