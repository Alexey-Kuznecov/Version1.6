

namespace UnityCommander.Abstractions.History
{
    public interface IUserFavoriteStore
    {
        IReadOnlyList<string> Load();

        void Save(IReadOnlyCollection<string> paths);
    }
}
