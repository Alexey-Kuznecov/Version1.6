
using System.Collections.Generic;
using System.Linq;
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
                        //DisplayMemberPath = "Name",
                        CellTemplateResourceKey = "ColumnNameDataTemplate",
                        Width = 200,
                        Order = 1,
                        SyncGroup = "Name",
                        ColumnValueHandler = f => ((BaseDirectory)f).Name
                    },
                    new ColumnModel
                    {
                        Id = "CreationTime",
                        Header = "Created",
                        //DisplayMemberPath = "CreationTime",
                        CellTemplateResourceKey = "ColumnCreationDateDataTemplate",
                        Width = 100,
                        Order = 2,
                        SyncGroup = "Created",
                        ColumnValueHandler = f => ((BaseDirectory)f).CreationTime
                    },
                    new ColumnModel
                    {
                        Id = "LastAccessTime",
                        Header = "Last Access",
                        //DisplayMemberPath = "LastAccessTime",
                        CellTemplateResourceKey = "ColumnLastAccessDateDataTemplate",
                        Width = 100,
                        Order = 3,
                        SyncGroup = "LastAccess",
                        ColumnValueHandler = f => ((BaseDirectory)f).LastAccessTime
                    },
                    new ColumnModel
                    {
                        Id = "Extension",
                        Header = "Ext",
                        //DisplayMemberPath = "Extension",
                        CellTemplateResourceKey = "ColumnExtensionDataTemplate",
                        Width = 80,
                        Order = 4,
                        SyncGroup = "Ext",
                        ColumnValueHandler = f => ((FileModel)f).Extension
                    },
                    new ColumnModel
                    {
                        Id = "Size",
                        Header = "Size",
                        //DisplayMemberPath = "Extension",
                        CellTemplateResourceKey = "ColumnFileSizeDataTemplate",
                        Width = 80,
                        Order = 5,
                        SyncGroup = "Size",
                        ColumnValueHandler = f => ((FileModel)f).Size
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
                        //DisplayMemberPath = "Name",
                        CellTemplateResourceKey = "ColumnNameDataTemplate",
                        Width = 200,
                        Order = 1,
                        SyncGroup = "Name",
                        ColumnValueHandler = f => ((BaseDirectory)f).Name
                    },
                    new ColumnModel
                    {
                        Id = "CreationTime",
                        Header = "Created",
                        //DisplayMemberPath = "CreationTime",
                        CellTemplateResourceKey = "ColumnCreationDateDataTemplate",
                        Width = 100,
                        Order = 2,
                        SyncGroup = "Created",
                        ColumnValueHandler = f => ((BaseDirectory)f).CreationTime
                    },
                    new ColumnModel
                    {
                        Id = "LastAccessTime",
                        Header = "Last Access",
                        //DisplayMemberPath = "LastAccessTime",
                        CellTemplateResourceKey = "ColumnLastAccessDateDataTemplate",
                        Width = 100,
                        Order = 3,
                        SyncGroup = "LastAccess",
                        ColumnValueHandler = f => ((BaseDirectory)f).LastAccessTime
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
                        SyncGroup = "Main",
                        ColumnValueHandler = f => ((DriveModel)f).Letter
                    },
                    new ColumnModel
                    {
                        Id = "Free Space",
                        Header = "Free Space",
                        CellTemplateResourceKey = "ColumnFreeSpaceDataTemplate",
                        Width = 100,
                        Order = 2,
                        SyncGroup = "Main",
                        ColumnValueHandler = f => ((DriveModel)f).FreeSpace
                    },
                    new ColumnModel
                    {
                        Id = "Used Space",
                        Header = "Used Space",
                        CellTemplateResourceKey = "ColumnUsedSpaceDataTemplate",
                        Width = 100,
                        Order = 3,
                        SyncGroup = "Main",
                        ColumnValueHandler = f => ((DriveModel)f).UsedSpace
                    },
                    new ColumnModel
                    {
                        Id = "Total Space",
                        Header = "Total Space",
                        CellTemplateResourceKey = "ColumnTotalSpaceDataTemplate",
                        Width = 100,
                        Order = 4,
                        SyncGroup = "Main",
                        ColumnValueHandler = f => ((DriveModel)f).TotalAmount
                    }
                };
            }

            return Enumerable.Empty<ColumnModel>();
        }
    }
}
