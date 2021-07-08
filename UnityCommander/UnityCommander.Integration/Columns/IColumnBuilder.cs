using System;
using System.Collections.Generic;
using System.Text;
using UnityCommander.Integration.Options;

namespace UnityCommander.Integration.Columns
{
    public interface IColumnBuilder
    {
        public void ColumnInitial(ColumnBuilder builder);
        
        public OptionRender ColumnValueRender();

        public object ColumnValueValidate(object value);

        public object ColumnValueHandler(object path);
    }
}
