
namespace UnityCommander.Common.Models.Columns
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;

    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Models.Base;

    /// <summary>
    /// The columns default.
    /// </summary>
    public abstract class DefaultColumns : IColumnService
    {
        /// <summary>
        /// The collection columns of the file panel.
        /// </summary>
        private ObservableCollection<IColumn> columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultColumns"/> class.
        /// </summary>
        protected DefaultColumns()
        {
            this.InitialData();
        }

        /// <summary>
        /// Gets or sets the column's name that display in settings.
        /// </summary>
        public string DisplayName { get; set; } = "Default";

        /// <summary>
        /// The set column value.
        /// </summary>
        /// <param name="yourModel">
        /// The your model.
        /// </param>
        /// <param name="currentPath">
        /// The current path.
        /// </param>
        public void SetColumnValue(Action<object> yourModel, string currentPath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Provides collection columns, only columns for which the IsDisplayed property is true.
        /// </summary>
        /// <param name="callback">
        /// The first parameter of callback function is column collection displayed in the file panel.
        /// The second parameter is a possible exception that can occur.
        /// </param>
        public void GetColumn(Action<ObservableCollection<IColumn>, Exception> callback)
        {
            callback(this.columns, null);
        }

        /// <summary>
        /// Safely adds a new column to the collection.
        /// </summary>
        /// <param name="column"> Add new column </param>
        public void AddColumn(IColumn column)
        {
            this.columns.Add(column);
        }

        /// <summary>
        /// Initial the collection of the column.
        /// </summary>
        protected void InitialData()
        {
            this.columns = new ObservableCollection<IColumn>
            {
                new BaseColumn
                {
                    Header = "Name",
                    IsDisplayed = true,
                    Template = new GridViewColumn
                    {
                        Header = "Name",
                        Width = 250,
                        CellTemplate = (DataTemplate)Application.Current.FindResource("ColumnNameDataTemplate")
                    }
                },
                 new BaseColumn
                {
                    Header = "CreationTime",
                    Template = new GridViewColumn
                    {
                        Header = "CreationTime",
                        Width = 150,
                        CellTemplate = (DataTemplate)Application.Current.FindResource("ColumnCreationDateDataTemplate")
                    }
                },
                new BaseColumn
                {
                    Header = "Last Access Time",
                    Template = new GridViewColumn
                    {
                        Header = "LastAccessTime",
                        Width = 150,
                        CellTemplate = (DataTemplate)Application.Current.FindResource("ColumnLastAccessDateDataTemplate")
                    },
                }
            };
        }
    }
}
