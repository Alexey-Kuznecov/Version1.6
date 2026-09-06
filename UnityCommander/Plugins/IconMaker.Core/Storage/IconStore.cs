
using IconMaker.Core.Models;

namespace IconMaker.Core.Storage
{
    public sealed class IconStore : IIconStore
    {
        private readonly IIconStorage _storage;

        private readonly Dictionary<string, IconPack> _cache = new();

        private readonly HashSet<string> _dirtyPacks = new();
        private readonly HashSet<string> _deletedPacks = new();

        public IconStore(IIconStorage storage)
        {
            _storage = storage;
        }

        // =========================
        // PACK LOADING
        // =========================

        public IconPack GetPack(string id)
        {
            if (_cache.TryGetValue(id, out var pack))
                return pack;

            pack = _storage.Load(id);

            if (pack == null)
                throw new InvalidOperationException($"Pack '{id}' not found.");

            _cache[id] = pack;

            return pack;
        }

        public IReadOnlyCollection<IconPack> GetLoadedPacks()
        {
            return _cache.Values.ToList().AsReadOnly();
        }

        // IconStore
        public IEnumerable<(string Id, string Name)> GetPackHeaders()
        {
            return _storage.GetPackHeaders();
        }

        // =========================
        // ICON OPERATIONS
        // =========================

        public void AddIcon(string packId, IconDefinition icon)
        {
            var pack = GetPack(packId);

            if (pack.Icons.Any(x => x.Id == icon.Id))
                throw new InvalidOperationException($"Icon '{icon.Id}' already exists in pack '{packId}'.");

            pack.AddIcon(icon);

            MarkDirty(packId);
        }

        public void RemoveIcon(string packId, Guid iconId)
        {
            var pack = GetPack(packId);

            var icon = pack.Icons.FirstOrDefault(x => x.Id == iconId);
            if (icon == null)
                return;

            pack.RemoveIcon(icon);

            MarkDirty(packId);
        }

        public void RenameIcon(string packId, Guid iconId, string newName)
        {
            var pack = GetPack(packId);

            var icon = pack.Icons.FirstOrDefault(x => x.Id == iconId);
            if (icon == null)
                return;

            icon.Name = newName;

            MarkDirty(packId);
        }

        // =========================
        // ADD PACK
        // =========================

        public void AddPack(IconPack pack)
        {
            _cache[pack.Id] = pack;

            _dirtyPacks.Add(pack.Id);

            _deletedPacks.Remove(pack.Id);
        }

        // =========================
        // REMOVE PACK
        // =========================

        public void RemovePack(string id)
        {
            if (_cache.Remove(id))
            {
                _dirtyPacks.Remove(id);
                _deletedPacks.Add(id);
            }
        }

        public void UpdateIcon(string packId, IconDefinition icon)
        {
            var pack = GetPack(packId);

            var existing = pack.Icons.FirstOrDefault(x => x.Id == icon.Id);
            if (existing == null)
            {
                pack.AddIcon(icon);
            }
            else
            {
                // обновляем поля (без замены ссылки)
                existing.Name = icon.Name;
                existing.Layers = icon.Layers;
                existing.Tags = icon.Tags;
            }

            MarkDirty(packId);
        }

        // =========================
        // SAVE LAYER
        // =========================

        public void SavePack(string id)
        {
            if (_deletedPacks.Contains(id))
            {
                _storage.Delete(id);
                _deletedPacks.Remove(id);
                return;
            }

            if (_cache.TryGetValue(id, out var pack))
            {
                _storage.Save(pack);
                _dirtyPacks.Remove(id);
            }
        }

        public void SaveAll()
        {
            foreach (var deleted in _deletedPacks.ToList())
            {
                _storage.Delete(deleted);
            }

            _deletedPacks.Clear();

            foreach (var dirty in _dirtyPacks.ToList())
            {
                if (_cache.TryGetValue(dirty, out var pack))
                {
                    _storage.Save(pack);
                }
            }

            _dirtyPacks.Clear();
        }

        // =========================
        // INTERNAL
        // =========================

        private void MarkDirty(string packName)
        {
            _dirtyPacks.Add(packName);
        }
    }
}
