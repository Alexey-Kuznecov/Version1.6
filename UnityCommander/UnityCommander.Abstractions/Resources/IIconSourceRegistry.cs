
namespace UnityCommander.Abstractions.Resources
{
    public interface IIconSourceRegistry
    {
        void Register(IIconSource source);

        IReadOnlyCollection<IIconSource> Sources { get; }
    }
}
