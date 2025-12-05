
namespace UnityCommander.Modules.FilePanel.Columns
{
    using System;
    using System.Collections.Generic;
    using UnityCommander.Common;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Options;

    public class CommonColumn : IColumn
    {
        public string Header { get; set; }
        public object Template { get; set; }
        public double Width { get; set; }
        public Action SortCommand { get; set; }
        public List<ContextItem> ContextItems { get; set; }
        public List<OptionBuilder> OptionBuilders { get; set; }
        public IColumnBuilder ColumnBuilder { get; set; }
        public ColumnManager ColumnManager { get; set; }
    }
}
