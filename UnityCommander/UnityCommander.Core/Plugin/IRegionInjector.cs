
namespace UnityCommander.Core.Plugin
{
    public interface IRegionInjector
    {
        void Inject(object host, string region, object view);
    }
}
