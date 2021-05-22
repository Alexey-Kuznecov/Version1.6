
namespace UnityCommander.Modules.FilePanel.Commands
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// The grid view sort.
    /// </summary>
    public class GridViewSort
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for PropertyName.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.RegisterAttached(
                "PropertyName",
                typeof(string),
                typeof(GridViewSort),
                new UIPropertyMetadata(null));

        #region Attached properties

        /// <summary>
        /// Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(GridViewSort),
                new UIPropertyMetadata(
                    null,
                    (o, e) =>
                    {
                        if (o is ItemsControl listView)
                        {
                            if (!GetAutoSort(listView)) // Don't change click handler if AutoSort enabled
                            {
                                if (e.OldValue != null && e.NewValue == null)
                                {
                                    listView.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                                }

                                if (e.OldValue == null && e.NewValue != null)
                                {
                                    listView.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                                }
                            }
                        }
                    }));

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutoSort.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AutoSortProperty =
            DependencyProperty.RegisterAttached(
                "AutoSort",
                typeof(bool),
                typeof(GridViewSort),
                new UIPropertyMetadata(
                    false,
                    (o, e) =>
                        {
                            if (o is ListView listView)
                            {
                                if (GetCommand(listView) == null) // Don't change click handler if a command is set
                                {
                                    bool oldValue = (bool)e.OldValue;
                                    bool newValue = (bool)e.NewValue;
                                    if (oldValue && !newValue)
                                    {
                                        listView.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                                    }

                                    if (!oldValue && newValue)
                                    {
                                        listView.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                                    }
                                }
                            }
                        }));

        #region Helper methods

        /// <summary>
        /// The get ancestor.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T GetAncestor<T>(DependencyObject reference) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(reference);
            while (!(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent ?? throw new InvalidOperationException());
            }

            return (T)parent;
        }

        /// <summary>
        /// The apply sort.
        /// </summary>
        /// <param name="view">
        /// The view.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        public static void ApplySort(ICollectionView view, string propertyName)
        {
            ListSortDirection direction = ListSortDirection.Ascending;
            if (view.SortDescriptions.Count > 0)
            {
                SortDescription currentSort = view.SortDescriptions[0];
                if (currentSort.PropertyName == propertyName)
                {
                    direction = currentSort.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                }
                view.SortDescriptions.Clear();
            }
            if (!string.IsNullOrEmpty(propertyName))
            {
                view.SortDescriptions.Add(new SortDescription(propertyName, direction));
            }
        }

        #endregion

        /// <summary>
        /// The get command.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// The <see cref="ICommand"/>.
        /// </returns>
        public static ICommand GetCommand(DependencyObject command)
        {
            return (ICommand)command.GetValue(CommandProperty);
        }

        /// <summary>
        /// Sets a custom command to sort the list into the object's command property,
        /// the object is a column from the file panel in our case.
        /// </summary>
        /// <param name="dependencyObject">
        /// Expected a column of the file panel.
        /// </param>
        /// <param name="command">
        /// The custom command is usually declared in the view model class.
        /// </param>
        public static void SetCommand(DependencyObject dependencyObject, ICommand command)
        {
            dependencyObject.SetValue(CommandProperty, command);
        }

        /// <summary>
        /// The get auto sort.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependencyObject.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetAutoSort(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(AutoSortProperty);
        }

        /// <summary>
        /// The set auto sort.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependencyObject.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetAutoSort(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(AutoSortProperty, value);
        }

        #endregion

        #region Column header click event handler

        /// <summary>
        /// The column header click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader headerClicked)
            {
                string propertyName = headerClicked.Column.Header.ToString();
                if (!string.IsNullOrEmpty(propertyName))
                {
                    ListView listView = GetAncestor<ListView>(headerClicked);
                    if (listView != null)
                    {
                        ICommand command = GetCommand(listView);
                        if (command != null)
                        {
                            if (command.CanExecute(propertyName))
                            {
                                command.Execute(propertyName);
                            }
                        }
                        else if (GetAutoSort(listView))
                        {
                            ApplySort(listView.Items, propertyName);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
