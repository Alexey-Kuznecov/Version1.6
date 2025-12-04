using System;
using System.Collections.Generic;
using UnityCommander.Common.Modules.Columns
namespace UnityCommander.Services.Interfaces
{
    public interface IColumnProvider
    {
        IEnumerable<ColumnDefinition> GetColumnDefinitions(PanelType panelType); // PanelType enum: Files, Folders, Drives...
    }
}
