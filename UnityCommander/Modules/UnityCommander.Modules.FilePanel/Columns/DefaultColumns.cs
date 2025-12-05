
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using UnityCommander.Integration.Columns;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public abstract class DefaultColumns 
    {
        private ObservableCollection<IColumn> columns;

        protected DefaultColumns()
        {
            this.InitialData();
        }

        public string DisplayName { get; set; } = "Default";

        public void GetColumn(Action<ObservableCollection<IColumn>, Exception> callback)
        {
            callback(this.columns, null);
        }

        public void AddColumn(IColumn column)
        {
            this.columns.Add(column);
        }

        protected void InitialData()
        {
            this.columns = new ObservableCollection<IColumn>
            {
                new CommonColumn
                {
                    Header = "Name",
                    Template = new GridViewColumn
                    {
                        Header = "Name",
                        Width = 200,
                        CellTemplate = (DataTemplate)Application.Current.FindResource("ColumnNameDataTemplate")
                    }
                },
                new CommonColumn
                {
                    Header = "CreationTime",
                    Template = new GridViewColumn
                    {
                        Header = "CreationTime",
                        Width = 100,
                        CellTemplate = (DataTemplate)Application.Current.FindResource("ColumnCreationDateDataTemplate")
                    }
                },
                new CommonColumn
                {
                    Header = "Last Access Time",
                    Template = new GridViewColumn
                    {
                        Header = "LastAccessTime",
                        Width = 100,
                        CellTemplate = (DataTemplate)Application.Current.FindResource("ColumnLastAccessDateDataTemplate")
                    },
                }
            };
        }
    }
}
