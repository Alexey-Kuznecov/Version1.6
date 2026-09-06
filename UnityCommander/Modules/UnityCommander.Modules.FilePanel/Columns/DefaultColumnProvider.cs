
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Abstractions.Columns;
using UnityCommander.Common.Models;
using UnityCommander.Common.Models.Directory;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public class DefaultColumnProvider : IColumnProvider
    {
        private readonly IFileStateService _fileStateService;

        public DefaultColumnProvider(IFileStateService fileStateService)
        {
            _fileStateService = fileStateService;
        }

        public IEnumerable<ColumnModel> GetColumnDefinitions(PanelType panelType)
        {
            if (panelType == PanelType.Files)
            {
                return new List<ColumnModel>
                {
                    new ColumnModel
                    {
                        Id = "core.name",
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
                        Id = "core.live",
                        Header = "Progess ##",
                        CellTemplateResourceKey = "ColumnTextDataTemplate",
                        Width = 100,
                        Order = 3,
                        SyncGroup = "Live",
                        RefreshInterval = 2000,
                        IsDynamic = true,
                        IsVisible = true,
                        ColumnValueHandler = f =>
                        {
                            var path = ((BaseDirectory)f).Path;

                            var state = (FileState)_fileStateService.GetState(path);

                            if (state != null)
                                return $"{state.Progress}%";

                            return $"(copied)";
                        }
                    },
                    new ColumnModel
                    {
                        Id = "core.creationTime",
                        Header = "Created",
                        //DisplayMemberPath = "CreationTime",
                        CellTemplateResourceKey = "ColumnCreationDateDataTemplate",
                        Width = 100,
                        Order = 6,
                        SyncGroup = "Created",
                        ColumnValueHandler = f => ((BaseDirectory)f).CreationTime
                    },
                    new ColumnModel
                    {
                        Id = "core.lastAccessTime",
                        Header = "Last Access",
                        //DisplayMemberPath = "LastAccessTime",
                        CellTemplateResourceKey = "ColumnLastAccessDateDataTemplate",
                        Width = 100,
                        Order = 7,
                        SyncGroup = "LastAccess",
                        ColumnValueHandler = f => ((BaseDirectory)f).LastAccessTime
                    },
                    new ColumnModel
                    {
                        Id = "core.extension",
                        Header = "Ext",
                        //DisplayMemberPath = "Extension",
                        CellTemplateResourceKey = "ColumnExtensionDataTemplate",
                        Width = 80,
                        Order = 8,
                        SyncGroup = "Ext",
                        ColumnValueHandler = f => ((FileModel)f).Extension
                    },
                    new ColumnModel
                    {
                        Id = "core.size",
                        Header = "Size",
                        //DisplayMemberPath = "Extension",
                        CellTemplateResourceKey = "ColumnFileSizeDataTemplate",
                        Width = 80,
                        Order = 5,
                        SyncGroup = "Size",
                        ColumnValueHandler = f => ((FileModel)f).Size
                    },
                    new ColumnModel
                    {
                        Id = "core.random",
                        Header = "Random (Core) ###",
                        //DisplayMemberPath = "LastAccessTime",
                        CellTemplateResourceKey = "ColumnTextDataTemplate",
                        UpdatePriority = ColumnUpdatePriority.Realtime,
                        Width = 100,
                        Order = 3,
                        SyncGroup = "Random",
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
                        Id = "core.name",
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
                        Id = "core.creationTime",
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
                        Id = "core.lastAccessTime",
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
                        Id = "core.random",
                        Header = "Random (Core) ###",
                        //DisplayMemberPath = "LastAccessTime",
                        CellTemplateResourceKey = "ColumnTextDataTemplate",
                        UpdatePriority = ColumnUpdatePriority.Background,
                        Width = 100,
                        Order = 3,
                        SyncGroup = "Random",
                        IsDynamic = true,
                        ColumnValueHandler = f =>
                        {
                            var r = Random.Shared.Next(100, 200);
                            return $"{r}%";
                        }
                    }
                };
            }

            if (panelType == PanelType.Drives)
            {
                return new List<ColumnModel>
                {
                    new ColumnModel
                    {
                        Id = "core.letter",
                        Header = "Letter",
                        CellTemplateResourceKey = "ColumnLetterDataTemplate",
                        Width = 100,
                        Order = 1,
                        SyncGroup = "Main",
                        ColumnValueHandler = f => ((DriveModel)f).Letter
                    },
                    new ColumnModel
                    {
                        Id = "core.freeSpace",
                        Header = "Free Space",
                        CellTemplateResourceKey = "ColumnFreeSpaceDataTemplate",
                        Width = 100,
                        Order = 2,
                        SyncGroup = "Main",
                        ColumnValueHandler = f => ((DriveModel)f).FreeSpace
                    },
                    new ColumnModel
                    {
                        Id = "core.usedSpace",
                        Header = "Used Space",
                        CellTemplateResourceKey = "ColumnUsedSpaceDataTemplate",
                        Width = 100,
                        Order = 3,
                        SyncGroup = "Main",
                        ColumnValueHandler = f => ((DriveModel)f).UsedSpace
                    },
                    new ColumnModel
                    {
                        Id = "core.totalSpace",
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
