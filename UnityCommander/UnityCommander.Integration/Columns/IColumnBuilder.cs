using System;
using System.Collections.Generic;
using System.Text;
using UnityCommander.Integration.Contracts;
using UnityCommander.Integration.Options;

namespace UnityCommander.Integration.Columns
{
    public interface IColumnBuilder
    {
        public void ColumnInitial(ColumnBuilder builder);
        
        public OptionRender ColumnValueRender();

        public object ColumnValueValidate(IPluginContext context);

        public object ColumnValueHandler(string path);
    }
}
