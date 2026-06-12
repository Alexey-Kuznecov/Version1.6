using IconMaker.Core.Models;

namespace IconMaker.Core.Storage
{
    public interface IIconStore
    {
        IconPack GetPack(string packId);

        IReadOnlyCollection<IconPack> GetLoadedPacks();

        public IEnumerable<string> GetPackIds();

        void AddPack(IconPack pack);

        void RemovePack(string packId);

        void AddIcon(string packId, IconDefinition icon);

        void RemoveIcon(string packId, Guid iconId);

        void RenameIcon(string packId, Guid iconId, string newName);

        void UpdateIcon(string packId, IconDefinition icon);

        void SavePack(string packIdv);

        void SaveAll();
    }
}
