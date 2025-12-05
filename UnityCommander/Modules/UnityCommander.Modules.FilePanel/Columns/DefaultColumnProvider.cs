using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UnityCommander.Common.Models.Directory;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public class DefaultColumnProvider : IColumnProvider
    {
        public IEnumerable<ColumnModel> GetColumnDefinitions(PanelType panelType)
        {
            if (panelType == PanelType.Files)
            {
                return new List<ColumnModel>
                {
                    new ColumnModel
                    {
                        Id = "Name",
                        Header = "Name",
                        CellTemplateResourceKey = "ColumnNameDataTemplate",
                        Width = 200,
                        Order = 1,
                        SyncGroup = "Main",
                        //ColumnValueHandler = f => f.Name
                    },
                    new ColumnModel
                    {
                        Id = "CreationTime",
                        Header = "Created",
                        CellTemplateResourceKey = "ColumnCreationDateDataTemplate",
                        Width = 100,
                        Order = 2,
                        SyncGroup = "Main",
                        //ColumnValueHandler = f => f.CreationTime
                    },
                    new ColumnModel
                    {
                        Id = "LastAccessTime",
                        Header = "Last Access",
                        CellTemplateResourceKey = "ColumnLastAccessDateDataTemplate",
                        Width = 100,
                        Order = 3,
                        SyncGroup = "Main",
                        //ColumnValueHandler = f => f.LastAccessTime
                    },
                    new ColumnModel
                    {
                        Id = "Extension",
                        Header = "Ext",
                        CellTemplateResourceKey = "ColumnExtensionDataTemplate",
                        Width = 80,
                        Order = 4,
                        SyncGroup = "Main",
                        //ColumnValueHandler = f => ((FileModel)f).Extension
                    }
                };
            }

            if (panelType == PanelType.Folders)
            {
                return new List<ColumnModel>
                {
                    new ColumnModel
                    {
                        Id = "Name",
                        Header = "Name",
                        CellTemplateResourceKey = "ColumnNameDataTemplate",
                        Width = 200,
                        Order = 1,
                        SyncGroup = "Main",
                        //ColumnValueHandler = f => f.Name
                    },
                    new ColumnModel
                    {
                        Id = "CreationTime",
                        Header = "Created",
                        CellTemplateResourceKey = "ColumnCreationDateDataTemplate",
                        Width = 100,
                        Order = 2,
                        SyncGroup = "Main",
                        //ColumnValueHandler = f => f.CreationTime
                    },
                    new ColumnModel
                    {
                        Id = "LastAccessTime",
                        Header = "Last Access",
                        CellTemplateResourceKey = "ColumnLastAccessDateDataTemplate",
                        Width = 100,
                        Order = 3,
                        SyncGroup = "Main",
                        //ColumnValueHandler = f => f.LastAccessTime
                    }
                };
            }

            if (panelType == PanelType.Drives)
            {
                return new List<ColumnModel>
                {
                    new ColumnModel
                    {
                        Id = "Letter",
                        Header = "Letter",
                        CellTemplateResourceKey = "ColumnLetterDataTemplate",
                        Width = 100,
                        Order = 1,
                        SyncGroup = "Main"
                    },
                    new ColumnModel
                    {
                        Id = "Free Space",
                        Header = "Free Space",
                        CellTemplateResourceKey = "ColumnFreeSpaceDataTemplate",
                        Width = 100,
                        Order = 2,
                        SyncGroup = "Main"
                    },
                    new ColumnModel
                    {
                        Id = "Used Space",
                        Header = "Used Space",
                        CellTemplateResourceKey = "ColumnUsedSpaceDataTemplate",
                        Width = 100,
                        Order = 3,
                        SyncGroup = "Main"
                    },
                    new ColumnModel
                    {
                        Id = "Total Space",
                        Header = "Total Space",
                        CellTemplateResourceKey = "ColumnTotalSpaceDataTemplate",
                        Width = 100,
                        Order = 4,
                        SyncGroup = "Main"
                    }
                };
            }

            return Enumerable.Empty<ColumnModel>();
        }
    }
}
