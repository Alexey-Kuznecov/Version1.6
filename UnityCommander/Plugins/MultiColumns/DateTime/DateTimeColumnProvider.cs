
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Common.Column;
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
                        Id = "debug.progress",
                        Header = "Debug",
                        //DisplayMemberPath = "Name",
                        CellTemplateResourceKey = "ColumnTextDataTemplate",
                        UpdatePriority = ColumnUpdatePriority.Realtime,
                        Width = 200,
                        Order = 1,
                        SyncGroup = "Name2",
                        IsDynamic = true,
                        ColumnValueHandler = f =>
                        {
                            var r = Random.Shared.Next(0, 100);
                            return $"{r}%";
                        }
                    },
                    new ColumnModel
                    {
                        Id = "debug.progress2",
                        Header = "Debug2",
                        //DisplayMemberPath = "Name",
                        CellTemplateResourceKey = "ColumnTextDataTemplate",
                        UpdatePriority = ColumnUpdatePriority.Normal,
                        Width = 200,
                        Order = 1,
                        SyncGroup = "Name2",
                        IsDynamic = true,
                        ColumnValueHandler = f =>
                        {
                            var r = Random.Shared.Next(100, 200);
                            return $"{r}%";
                        }
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
                        CellTemplateResourceKey = "ColumnTextDataTemplate",
                        UpdatePriority = ColumnUpdatePriority.Background,
                        Width = 100,
                        Order = 3,
                        SyncGroup = "LastAccess",
                        IsDynamic = true,
                        ColumnValueHandler = f =>
                        {
                            var r = Random.Shared.Next(0, 100);
                            return $"{r}%";
                        }
                    }
                };
            }

            return Enumerable.Empty<ColumnModel>();
        }
    }
}
