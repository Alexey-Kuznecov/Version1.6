
namespace UnityCommander.Rendering.Icons
{
    public interface IIconSourceRegistry
    {
        void Register(IIconSource source);

        IReadOnlyCollection<IIconSource> Sources { get; }
    }
}
