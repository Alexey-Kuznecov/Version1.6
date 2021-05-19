
namespace UnityCommander.Common.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using UnityCommander.Integration.Contracts;

    /// <summary>
    /// The columns default.
    /// </summary>
    public class ColumnsDate : IColumnService
    {
        /// <summary>
        /// The columns.
        /// </summary>
        private ObservableCollection<IColumn> columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnsDate"/> class.
        /// </summary>
        public ColumnsDate()
        {
            this.InitialData();
        }

        /// <summary>
        /// Gets or sets the column title.
        /// </summary>
        public string Title { get; set; } = "Dates";

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
        /// The get creation time.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object GetCreationTime(string path)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                return new { CreationTime = directoryInfo.CreationTime.ToLongDateString() };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
                    Header = "CreationTime",
                    Action = this.GetCreationTime,
                    ColumnTemplate = new GridViewColumn
                    {
                        Header = "CreationTime",
                        Width = 150,
                        CellTemplate = (DataTemplate)Application.Current.FindResource("ColumnDateDataTemplate")
                    }
                },
                new ColumnModel
                {
                    Header = "Last Access Time",
                    ColumnTemplate = new GridViewColumn
                    {
                        Header = "LastAccessTime",
                        Width = 100,
                        DisplayMemberBinding = new Binding("LastAccessTime"),
                        CellTemplate = (DataTemplate)Application.Current.FindResource("ColumnDateDataTemplate")
                    }
                }
            };
        }
    }
}
