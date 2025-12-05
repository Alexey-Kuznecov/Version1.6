
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UnityCommander.Modules.FilePanel.Columns;

namespace UnityCommander.Modules.FilePanel.Behaviors
{
    /// <summary>
    /// Поведение для ListView с GridView, позволяющее задавать колонки через коллекцию ColumnModel.
    /// Сюда включена поддержка:
    /// - создание и обновление колонок на основе ColumnModel
    /// - биндинги DisplayMemberPath или CellTemplate
    /// - синхронизация ширины колонок через SyncGroup
    /// </summary>
    public static class GridViewColumnsBehavior
    {
        #region Attached Properties

        // Основная коллекция колонок, привязанная к ListView
        public static readonly DependencyProperty ColumnDefinitionsProperty =
            DependencyProperty.RegisterAttached(
                "ColumnDefinitions",
                typeof(IEnumerable<ColumnModel>),
                typeof(GridViewColumnsBehavior),
                new PropertyMetadata(null, OnColumnDefinitionsChanged));

        public static void SetColumnDefinitions(DependencyObject d, IEnumerable<ColumnModel> value)
            => d.SetValue(ColumnDefinitionsProperty, value);

        public static IEnumerable<ColumnModel> GetColumnDefinitions(DependencyObject d)
            => (IEnumerable<ColumnModel>)d.GetValue(ColumnDefinitionsProperty);

        // Идентификатор панели (не используется внутри этого класса, но можно для внешней логики)
        public static readonly DependencyProperty PanelIdProperty =
            DependencyProperty.RegisterAttached(
                "PanelId",
                typeof(string),
                typeof(GridViewColumnsBehavior),
                new PropertyMetadata(null));

        public static void SetPanelId(DependencyObject d, string value)
            => d.SetValue(PanelIdProperty, value);

        public static string GetPanelId(DependencyObject d)
            => (string)d.GetValue(PanelIdProperty);

        // Словарь для хранения соответствия ColumnModel.Id -> GridViewColumn
        private static readonly DependencyProperty ColumnMapProperty =
            DependencyProperty.RegisterAttached(
                "ColumnMap",
                typeof(Dictionary<string, GridViewColumn>),
                typeof(GridViewColumnsBehavior),
                new PropertyMetadata(null));

        private static void SetColumnMap(DependencyObject d, Dictionary<string, GridViewColumn> value)
            => d.SetValue(ColumnMapProperty, value);

        private static Dictionary<string, GridViewColumn> GetColumnMap(DependencyObject d)
            => (Dictionary<string, GridViewColumn>)d.GetValue(ColumnMapProperty);

        // Словарь для хранения обработчиков изменения ширины колонок
        private static readonly DependencyProperty WidthHandlersProperty =
            DependencyProperty.RegisterAttached(
                "WidthHandlers",
                typeof(Dictionary<string, EventHandler>),
                typeof(GridViewColumnsBehavior),
                new PropertyMetadata(null));

        private static void SetWidthHandlers(DependencyObject d, Dictionary<string, EventHandler> value)
            => d.SetValue(WidthHandlersProperty, value);

        private static Dictionary<string, EventHandler> GetWidthHandlers(DependencyObject d)
            => (Dictionary<string, EventHandler>)d.GetValue(WidthHandlersProperty);

        // Словарь для хранения обработчиков синхронизации ширины колонок
        private static readonly DependencyProperty SyncHandlersProperty =
            DependencyProperty.RegisterAttached(
                "SyncHandlers",
                typeof(Dictionary<string, Action<double>>),
                typeof(GridViewColumnsBehavior),
                new PropertyMetadata(null));

        private static void SetSyncHandlers(DependencyObject d, Dictionary<string, Action<double>> value)
            => d.SetValue(SyncHandlersProperty, value);

        private static Dictionary<string, Action<double>> GetSyncHandlers(DependencyObject d)
            => (Dictionary<string, Action<double>>)d.GetValue(SyncHandlersProperty);

        #endregion

        /// <summary>
        /// Срабатывает, когда коллекция ColumnDefinitions изменяется.
        /// Здесь создаются новые колонки, обновляются существующие, удаляются ненужные.
        /// </summary>
        private static void OnColumnDefinitionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Проверяем, что объект – ListView
            if (d is not ListView listView) return;

            // Берём новые определения колонок, фильтруем видимые и сортируем по Order
            var defs = (e.NewValue as IEnumerable<ColumnModel>)?.Where(x => x.IsVisible).OrderBy(x => x.Order).ToList()
                        ?? new List<ColumnModel>();

            // Получаем или создаём GridView
            if (!(listView.View is GridView gridView))
            {
                gridView = new GridView();
                listView.View = gridView;
            }

            // Получаем словари, которые хранят соответствия колонок и обработчики
            var map = GetColumnMap(listView) ?? new Dictionary<string, GridViewColumn>();
            var widthHandlers = GetWidthHandlers(listView) ?? new Dictionary<string, EventHandler>();
            var syncHandlers = GetSyncHandlers(listView) ?? new Dictionary<string, Action<double>>();

            // Быстрый доступ к ColumnModel по Id
            var modelLookup = defs.ToDictionary(m => m.Id, m => m);

            // Удаляем колонки, которых больше нет в модели
            var toRemove = map.Keys.Except(modelLookup.Keys).ToList();
            foreach (var id in toRemove)
            {
                if (map.TryGetValue(id, out var oldCol))
                {
                    // Снимаем обработчик изменения ширины
                    if (widthHandlers.TryGetValue(id, out var handler))
                    {
                        var dpd = DependencyPropertyDescriptor.FromProperty(GridViewColumn.WidthProperty, typeof(GridViewColumn));
                        dpd?.RemoveValueChanged(oldCol, handler);
                        widthHandlers.Remove(id);
                    }

                    // Снимаем обработчик синхронизации
                    if (syncHandlers.TryGetValue(id, out var syncHandler))
                    {
                        // В реальной архитектуре ColumnSyncService не должен быть ресурсом!
                        // Здесь оставлено для совместимости со старым кодом
  
                        ColumnSyncService.UnregisterHandler(modelLookup.ContainsKey(id) ? modelLookup[id].SyncGroup : null, syncHandler);
                        syncHandlers.Remove(id);
                    }

                    // Удаляем колонку из GridView
                    var existing = gridView.Columns.FirstOrDefault(c => ReferenceEquals(c, oldCol));
                    if (existing != null) gridView.Columns.Remove(existing);

                    map.Remove(id);
                }
            }

            int insertIndex = 0;

            foreach (var def in defs)
            {
                if (map.TryGetValue(def.Id, out var existingCol))
                {
                    // Обновляем существующую колонку
                    existingCol.Header = def.Header;

                    if (Math.Abs(existingCol.Width - def.Width) > 0.01)
                        existingCol.Width = def.Width;

                    if (!string.IsNullOrEmpty(def.DisplayMemberPath))
                        existingCol.DisplayMemberBinding = new Binding(def.DisplayMemberPath);
                    else if (!string.IsNullOrEmpty(def.CellTemplateResourceKey))
                    {
                        var template = listView.TryFindResource(def.CellTemplateResourceKey) as DataTemplate
                                        ?? Application.Current.TryFindResource(def.CellTemplateResourceKey) as DataTemplate;
                        if (template != null) existingCol.CellTemplate = template;
                    }

                    // Обеспечиваем правильный порядок колонок
                    var currIndex = gridView.Columns.IndexOf(existingCol);
                    if (currIndex != insertIndex)
                    {
                        gridView.Columns.RemoveAt(currIndex);
                        gridView.Columns.Insert(insertIndex, existingCol);
                    }
                }
                else
                {
                    // Создаём новую колонку
                    var col = new GridViewColumn { Header = def.Header, Width = def.Width };

                    if (!string.IsNullOrEmpty(def.DisplayMemberPath))
                        col.DisplayMemberBinding = new Binding(def.DisplayMemberPath);
                    else if (!string.IsNullOrEmpty(def.CellTemplateResourceKey))
                    {
                        var template = listView.TryFindResource(def.CellTemplateResourceKey) as DataTemplate
                                        ?? Application.Current.TryFindResource(def.CellTemplateResourceKey) as DataTemplate;
                        if (template != null) col.CellTemplate = template;
                    }

                    gridView.Columns.Insert(insertIndex, col);
                    map[def.Id] = col;

                    // Добавляем обработчик изменения ширины колонки
                    var dpd = DependencyPropertyDescriptor.FromProperty(GridViewColumn.WidthProperty, typeof(GridViewColumn));
                    if (dpd != null)
                    {
                        EventHandler handler = (s, args) =>
                        {
                            if (s is GridViewColumn gcol)
                            {
                                var pair = map.FirstOrDefault(x => ReferenceEquals(x.Value, gcol));
                                if (!string.IsNullOrEmpty(pair.Key) && modelLookup.TryGetValue(pair.Key, out var model))
                                {
                                    model.Width = gcol.Width;

                                    // Синхронизация ширины через SyncGroup
                                    if (!string.IsNullOrEmpty(model.SyncGroup))
                                    {
                                        try
                                        {
                                            ColumnSyncService.NotifyWidthChanged(model.SyncGroup, gcol.Width);
                                        }
                                        catch { }
                                    }
                                }
                            }
                        };
                        dpd.AddValueChanged(col, handler);
                        widthHandlers[def.Id] = handler;

                        // Чистка обработчиков при выгрузке
                        listView.Unloaded += (_, __) => dpd.RemoveValueChanged(col, handler);
                    }

                    // Регистрация синхронизации ширины
                    if (!string.IsNullOrEmpty(def.SyncGroup))
                    {
                        Action<double> applyWidth = newWidth =>
                        {
                            if (map.TryGetValue(def.Id, out var gcol))
                            {
                                if (Math.Abs(gcol.Width - newWidth) > 0.5)
                                    Application.Current.Dispatcher.Invoke(() => gcol.Width = newWidth);
                            }
                        };

           
                        ColumnSyncService.RegisterHandler(def.SyncGroup, applyWidth);
                        syncHandlers[def.Id] = applyWidth;
                    }
                }

                insertIndex++;
            }

            // Сохраняем словари обратно в attached properties
            SetColumnMap(listView, map);
            SetWidthHandlers(listView, widthHandlers);
            SetSyncHandlers(listView, syncHandlers);
        }
    }
}
