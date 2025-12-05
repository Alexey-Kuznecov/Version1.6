using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public class DriveColumnsProvider : IColumnProvider
    {
        public IEnumerable<ColumnModel> GetColumnDefinitions(PanelType panelType)
        {
            if (panelType != PanelType.Drives)
                yield break;

            yield return new ColumnModel
            {
                Id = "Letter",
                Header = "Letter",
                CellTemplateResourceKey = "ColumnLetterDataTemplate",
                Width = 100,
                Order = 1,
                SyncGroup = "Main",
                DisplayMemberPath = "Name", // или что там у тебя
                ForDrives = true,
            };

            yield return new ColumnModel
            {
                Id = "Free Space",
                Header = "Free Space",
                CellTemplateResourceKey = "ColumnFreeSpaceDataTemplate",
                Width = 100,
                Order = 2,
                SyncGroup = "Main",
                DisplayMemberPath = "TotalSize",
                ForDrives = true,
            };

            yield return new ColumnModel
            {
                Id = "Used Space",
                Header = "Used Space",
                CellTemplateResourceKey = "ColumnUsedSpaceDataTemplate",
                Width = 100,
                Order = 3,
                SyncGroup = "Main",
                DisplayMemberPath = "TotalSize",
                ForDrives = true,
            };

            yield return new ColumnModel
            {
                Id = "Total Space",
                Header = "Total Space",
                CellTemplateResourceKey = "ColumnTotalSpaceDataTemplate",
                Width = 100,
                Order = 4,
                SyncGroup = "Main",
                DisplayMemberPath = "TotalSize",
                ForDrives = true,
            };
        }
    }
}
