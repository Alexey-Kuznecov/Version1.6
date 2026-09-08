
namespace UnityCommander.Abstractions.History
{
    public interface IUserFavorites
    {
        IReadOnlyList<string> Paths { get; }

        void Add(string path);
        void Remove(string path);
        void Clear();
    }
}
