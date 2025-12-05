using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Common.Models.Directory;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public class ColumnModel
    {
        public string Id { get; init; }                // уникальный идентификатор
        public string Header { get; init; }
        public string CellTemplateResourceKey { get; init; } // ключ DataTemplate
        public string DisplayMemberPath { get; init; } // простой биндинг
        public double Width { get; set; }              // текущая ширина
        public bool IsVisible { get; set; } = true;
        public int Order { get; init; }               // порядок отображения
        public string SyncGroup { get; init; }         // для синхронизации ширины
        public bool ForFiles { get; init; } = true;
        public bool ForFolders { get; init; } = true;
        public bool ForDrives { get; init; } = true;
        public Func<BaseDirectory, object> ColumnValueHandler { get; set; }
    }
}
