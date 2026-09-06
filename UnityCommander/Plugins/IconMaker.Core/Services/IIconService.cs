using IconMaker.Core.Models;

namespace IconMaker.Core.Services
{
    public interface IIconService
    {
        public event Action<string>? PackChanged;
        public event Action<string>? PackRemoved;

        public event Action<string, Guid>? IconRemoved;
        public event Action<string, Guid>? IconUpdated;

        // PACK OPERATIONS
        IconPack GetPack(string packId);

        IReadOnlyList<IconPack> GetAllPacks();

        IEnumerable<(string Id, string Name)> GetPackHeaders();

        void CreatePack(string id, string name);

        void ImportPack(IconPack pack);

        void DeletePack(string id);

        void RenamePack(string oldName, string newName);

        // ICON OPERATIONS (explicit target pack)
        void AddIcon(string packId, IconDefinition icon);

        void RemoveIcon(string packId, Guid iconId);

        void RenameIcon(string packId, Guid iconId, string newName);

        void UpdateIcon(string packId, IconDefinition icon);

        // PERSISTENCE
        void SavePack(string packId);

        void SaveAll();
    }
}
