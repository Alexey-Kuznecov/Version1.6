
namespace UnityCommander.Common.Models
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using UnityCommander.Integration.Contracts;

    /// <summary>
    /// The columns default.
    /// </summary>
    public class ColumnsDefault : IColumnService
    {
        /// <summary>
        /// The columns.
        /// </summary>
        private ObservableCollection<IColumn> columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnsDefault"/> class.
        /// </summary>
        public ColumnsDefault()
        {
            this.InitialData();
        }

        /// <summary>
        /// Gets or sets the column title.
        /// </summary>
        public string Title { get; set; } = "Default";

        /// <summary>
        /// The get column.
        /// </summary>
        /// <param name="callback">
        /// The <paramref name="callback"/>.
        /// </param>
        public void GetColumn(Action<ObservableCollection<IColumn>, Exception> callback)
        {
            callback(this.columns, null);
        }

        /// <summary>
        /// The initial data.
        /// </summary>
        private void InitialData()
        {
            this.columns = new ObservableCollection<IColumn>
            {
                new ColumnModel
                {
                    Header = "Name",
                    IsChecked = true,
                    ColumnTemplate = new GridViewColumn
                    {
                        Header = "Name",
                        Width = 250,
                        CellTemplate = (DataTemplate)Application.Current.FindResource("ColumnNameDataTemplate")
                    }
                },
                new ColumnModel
                {
                    Header = "Extension",
                    IsChecked = true,
                    ColumnTemplate = new GridViewColumn
                    {
                        Header = "Extension",
                        Width = 150,
                        CellTemplate = (DataTemplate)Application.Current.FindResource("ColumnExtensionDataTemplate")
                    }
                }
            };
        }
    }
}
