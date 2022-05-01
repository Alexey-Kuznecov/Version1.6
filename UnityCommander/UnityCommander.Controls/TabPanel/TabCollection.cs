
namespace UnityCommander.Controls.TabPanel
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Controls;

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

        public Control Active { get; set; }

        #region Properties

        /// <summary>
        /// Gets or sets value for the item by that index
        /// </summary>
        /// <param name="index">
        /// The tab control index,
        /// </param>
        /// <returns>
        /// The tab control at the specified index.
        /// </returns>
        public Control this[int index]
        {
            get => (Control)this.List[index];
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
        public int IndexOf(Control control)
        {
            if (control != null)
            {
                return this.List.IndexOf(control);
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
        public int Add(Control control)
        {
            if (control != null)
            {
                var position = this.List.Add(control);
                this.RaiseEvent();
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
        public void Remove(Control control)
        {
            this.List.Remove(control);
            this.RaiseEvent();
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
        public void Insert(int index, Control control)
        {
            if (index <= this.List.Count && control != null)
            {
                this.List.Insert(index, control);
                this.RaiseEvent();
            }
        }

        /// <summary>
        /// The get active.
        /// </summary>
        /// <returns>
        /// The <see cref="Control"/>.
        /// </returns>
        public Control GetActive()
        {
            byte counter = 0;
            TabCollection collection = (TabCollection)this.List;
            Control control;

            do
            {
                control = collection[counter];
                counter++;
            } 
            while (control.IsEnabled);

            this.Active = control;

            return control;
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
        /// </summary>>
        private void RaiseEvent()
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged.Invoke(this, new CollectionChangedEventArg((TabCollection)this.List));
            }
        }
    }
}
