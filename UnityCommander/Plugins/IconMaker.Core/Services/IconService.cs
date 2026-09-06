
using IconMaker.Core.Models;
using IconMaker.Core.Storage;

namespace IconMaker.Core.Services
{
    public sealed class IconService : IIconService
    {
        private readonly IIconStore _store;

        public event Action<string>? PackChanged;
        public event Action<string>? PackRemoved;

        public event Action<string, Guid>? IconRemoved;
        public event Action<string, Guid>? IconUpdated;

        public IconService(IIconStore store)
        {
            _store = store;
        }

        // =========================
        // PACK OPERATIONS
        // =========================

        public IconPack GetPack(string packId)
        {
            return _store.GetPack(packId);
        }

        public IReadOnlyList<IconPack> GetAllPacks()
        {
            return _store.GetLoadedPacks().ToList();
        }

        public IEnumerable<(string Id, string Name)> GetPackHeaders() 
        { 
            return _store.GetPackHeaders(); 
        }

        public void ImportPack(IconPack pack)
        {
            _store.AddPack(pack);
        }

        public void CreatePack(string packId, string name)
        {
            var pack = new IconPack(packId, name);

            _store.AddPack(pack);
        }

        public void DeletePack(string packId)
        {
            _store.RemovePack(packId);
        }

        public void RenamePack(string packId, string newName)
        {
            var pack = _store.GetPack(packId);

            _store.RemovePack(packId);

            pack.Name = newName;

            _store.AddPack(pack);

            PackChanged?.Invoke(packId);
        }

        // =========================
        // ICON OPERATIONS
        // =========================

        public void AddIcon(string packId, IconDefinition icon)
        {
            _store.AddIcon(packId, icon);
        }

        public void RemoveIcon(string packId, Guid iconId)
        {
            _store.RemoveIcon(packId, iconId);
            
            IconRemoved?.Invoke(packId, iconId);
        }

        public void RenameIcon(string packId, Guid iconId, string newName)
        {
            _store.RenameIcon(packId, iconId, newName);
            
            IconUpdated?.Invoke(packId, iconId);
        }

        public void UpdateIcon(string packId, IconDefinition icon)
        {
            _store.UpdateIcon(packId, icon);
            
            IconUpdated?.Invoke(packId, icon.Id);
        }

        // =========================
        // PERSISTENCE
        // =========================

        public void SavePack(string packId)
        {
            _store.SavePack(packId);
        }

        public void SaveAll()
        {
            _store.SaveAll();
        }
    }
}
