
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UnityCommander.Modules.FilePanel.Columns;

namespace UnityCommander.Modules.FilePanel.Behaviors
{
    public static class GridViewColumnsBehavior
    {
        public static readonly DependencyProperty ColumnDefinitionsProperty =
            DependencyProperty.RegisterAttached("ColumnDefinitions",
                typeof(IEnumerable<ColumnModel>),
                typeof(GridViewColumnsBehavior),
                new PropertyMetadata(null, OnColumnDefinitionsChanged));

        public static void SetColumnDefinitions(DependencyObject d, IEnumerable<ColumnModel> value)
            => d.SetValue(ColumnDefinitionsProperty, value);

        public static IEnumerable<ColumnModel> GetColumnDefinitions(DependencyObject d)
            => (IEnumerable<ColumnModel>)d.GetValue(ColumnDefinitionsProperty);

        private static void OnColumnDefinitionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ListView listView) return;

            var defs = e.NewValue as IEnumerable<ColumnModel> ?? Enumerable.Empty<ColumnModel>();
            var gridView = new GridView();

            foreach (var def in defs)
            {
                var col = new GridViewColumn { Header = def.Header, Width = def.Width };

                if (!string.IsNullOrEmpty(def.DisplayMemberPath))
                    col.DisplayMemberBinding = new Binding(def.DisplayMemberPath);
                else if (!string.IsNullOrEmpty(def.CellTemplateResourceKey))
                {
                    var template = listView.TryFindResource(def.CellTemplateResourceKey) as DataTemplate
                                   ?? Application.Current.TryFindResource(def.CellTemplateResourceKey) as DataTemplate;
                    if (template != null) col.CellTemplate = template;
                }

                gridView.Columns.Add(col);
            }

            listView.View = gridView;
        }
    }
}
