
using System;
using System.Collections.Generic;
using UnityCommander.Integration.Contracts;
using UnityCommander.Integration.Options;

namespace UnityCommander.Integration.Columns
{
    using Enums;

    public class ColumnBuilder
    {
        private Column column;

        public void Add(string header, double width, TargetPanel target = TargetPanel.All)
        {
            this.column = new Column
            {
                Header = header,
                Width = width,
                TargetPanel = target
            };
        }

        public void AddCommand(Action action)
        {
            column.SortCommand = action;
        }

        public void BindOption(Type source, string propertyName, Selector handler)
        {

        }

        public void AddContextItem(string header, Action action)
        {
            column.ContextItems.Add(new ContextItem
            {
                Name = header,
                Command = action
            });
        }

        public Column GetColumn()
        {
            return column;
        }
    }
}
