
namespace UnityCommander.Controls.Taber
{
    using System;
    using System.Collections;
    using System.Windows;

    public class TabCollection : CollectionBase
    {
        public event EventHandler CollectionChanged;

        protected virtual void OnCollectionChangedEvent(EventArgs e)
        {
            EventHandler handler = CollectionChanged;
            handler?.Invoke(this, e);
        }

        #region Properties
        /// <summary>
        /// Gets/Sets value for the item by that index
        /// </summary>
        public TaberControl this[int index]
        {
            get => (TaberControl)this.List[index];
            set => this.List[index] = value;
        }

        #endregion

        #region Public Methods

        public int IndexOf(TaberControl control)
        {
            if (control != null)
            {
                return base.List.IndexOf(control);
            }
            return -1;
        }

        public int Add(TaberControl control)
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

        public void Remove(TaberControl control)
        {
            control.TabClick -= this.OnControlTabClick;
            this.InnerList.Remove(control);
            this.RaiseEvent(this);
        }

        public void AddRange(TabCollection collection)
        {
            if (collection != null)
            {
                this.InnerList.AddRange(collection);
            }
        }

        public void Insert(int index, TaberControl control)
        {
            if (index <= List.Count && control != null)
            {
                this.List.Insert(index, control);
                this.RaiseEvent(this);
            }
        }

        public bool Contains(TaberControl control)
        {
            return this.List.Contains(control);
        }

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
            var control = (TaberControl)sender;
            var panel = (e as TabCommandExecutedEventArg)?.Content as TaberPanel;

            if ((string)control.Content == "+")
                return;

            foreach (TaberControl item in this.List)
            {
                if ((string)item.Content == "+")
                    continue;
                if (item.Equals(control))
                    item.IsEnabled = false;
                else
                    item.IsEnabled = true;
            }
        }

        #endregion
    }
}
