
namespace UnityCommander.Abstractions.Plugin
{
    public interface ICompositionBuilder
    {
        void Add<TView, TViewModel>(string region = null);
    }
}
