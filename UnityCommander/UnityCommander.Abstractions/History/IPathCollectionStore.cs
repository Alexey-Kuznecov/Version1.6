
namespace UnityCommander.Abstractions.History
{
    public interface IUserNavigationHistoryStore
    {
        IReadOnlyList<string> Load();

        void Save(IReadOnlyCollection<string> paths);
    }
}
