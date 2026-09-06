
namespace UnityCommander.Abstractions.Icons
{
    public interface IIconSourceRegistry
    {
        void Register(IIconSource source);

        IReadOnlyCollection<IIconSource> Sources { get; }
    }
}
