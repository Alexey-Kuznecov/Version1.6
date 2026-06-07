
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Modules.FilePanel.States;

namespace UnityCommander.Modules.FilePanel.Controllers
{
    public class ColumnController<T> where T : BaseDirectory
    {
        private readonly ColumnRegistry _registry;
        private readonly IColumnStateManager _state;

        private readonly ObservableCollection<T> _source;
        
        private readonly PanelType _panelType;

        public ObservableCollection<ColumnModel> Columns { get; } = new();

        public ColumnController(
            IColumnSource<T> source,
            ColumnRegistry registry,
            IColumnStateManager state)
        {
            _source = source.Items;
            _registry = registry;
            _state = state;
        }

        public void Build(string key, PanelType type)
        {
            Columns.Clear();

            var defs = _registry.GetColumns(type).ToList();

            var loaded = _state.LoadState(key, type, defs);

            foreach (var c in loaded)
                Columns.Add(c);
        }

        public async Task UpdateValuesAsync(PanelType type)
        {
            var defs = _registry.GetColumns(type).ToList();

            await Task.Run(() =>
            {
                foreach (var item in _source)
                {
                    foreach (var col in defs)
                    {
                        var value = col.ColumnValueHandler(item);
                        item.Additional[col.Id] = value;
                    }
                }
            });
        }
    }
}
