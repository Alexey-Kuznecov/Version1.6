
namespace DateTimeColumns
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Enums;

    /// <summary>
    /// The home library book service.
    /// </summary>
    [Export(typeof(IColumnService))]
    public class DateTime : IColumnService
    {
        /// <summary>
        /// The columns.
        /// </summary>
        private ObservableCollection<IColumn> columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTime"/> class.
        /// </summary>
        public DateTime()
        {
           this.InitialData();
        }

        /// <summary>
        /// Gets or sets the column title.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The get column.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public void GetColumn(Action<ObservableCollection<IColumn>, Exception> callback)
        {
            callback(this.columns, null);
        }

        /// <summary>
        /// The set column value.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="currentPath">
        /// The current path.
        /// </param>
        public void SetColumnValue(Action<object, TargetPanel> model, string currentPath)
        {
            var dateTimeModel = new DateTimeModel();
            DirectoryInfo directoryInfo = new DirectoryInfo(currentPath);
            dateTimeModel.CreationTime = directoryInfo.CreationTime;
            dateTimeModel.LastAccessTime = directoryInfo.LastAccessTime;
            model(dateTimeModel, TargetPanel.Files);
        }

        /// <summary>
        /// The initial data.
        /// </summary>
        private void InitialData()
        {
            this.columns = new ObservableCollection<IColumn>
            {
                new DateTimeColumnModel
                {
                    IsDisplayed = false,
                    Header = nameof(DateTimeModel.CreationTime),
                    Template = new GridViewColumn
                    {
                        Header = nameof(DateTimeModel.CreationTime),
                        Width = 150,
                        DisplayMemberBinding =
                            new Binding { Path = new PropertyPath(nameof(DateTimeModel.CreationTime)) }
                    }
                },
                new DateTimeColumnModel
                {
                    IsDisplayed = false,
                    Header = nameof(DateTimeModel.LastAccessTime),
                    Template = new GridViewColumn
                    {
                        Header = nameof(DateTimeModel.LastAccessTime),
                        Width = 100,
                        DisplayMemberBinding = new Binding(nameof(DateTimeModel.LastAccessTime))
                    }
                }
            };
        }
    }
}
