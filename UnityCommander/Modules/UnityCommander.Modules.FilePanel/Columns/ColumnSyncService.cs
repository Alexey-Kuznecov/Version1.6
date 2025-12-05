
using System;
using System.Collections.Generic;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public class ColumnSyncService
    {
        private readonly ISettingsStore settings;

        public event Action<string /*syncGroup*/, ColumnModel /*new value*/> ColumnChanged;

        public ColumnSyncService(ISettingsStore settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Вызывается при изменении ширины колонки. Сохраняет и рассылает событие другим колонкам с тем же SyncGroup.
        /// </summary>
        public void NotifyWidthChanged(string syncGroup, ColumnModel changed)
        {
            if (changed == null) return;

            settings.SaveColumnWidth(changed.Id, changed.Width);
            ColumnChanged?.Invoke(syncGroup, changed);
        }

        /// <summary>
        /// Восстанавливает ширины колонок из настроек
        /// </summary>
        public void RestoreWidths(List<ColumnModel> columns)
        {
            if (columns == null) return;

            foreach (var col in columns)
            {
                var width = settings.LoadColumnWidth(col.Id);
                if (width.HasValue)
                    col.Width = width.Value;
            }
        }
    }
}
