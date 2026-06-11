
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Common.Columns;
using UnityCommander.Common.Models.Directory;

namespace MultiColumns.DateTime
{
    internal class DateTimeColumnProvider : IColumnProvider
    {
        public IEnumerable<ColumnModel> GetColumnDefinitions(PanelType panelType)
        {
            if (panelType == PanelType.Files)
            {
                return new List<ColumnModel>
                {
                    new ColumnModel
                    {
                        Id = "multi.name",
                        Header = "Name",
                        //DisplayMemberPath = "Name",
                        CellTemplateResourceKey = "ColumnNameDataTemplate",
                        Width = 200,
                        Order = 1,
                        SyncGroup = "Name",
                        ColumnValueHandler = f => ((BaseDirectory)f).Name
                    }
                };
            }
            if (panelType == PanelType.Folders)
            {
                return new List<ColumnModel>
                {
                    new ColumnModel
                    {
                        Id = "multi.lastAccessTime",
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

            return Enumerable.Empty<ColumnModel>();
        }
    }
}
