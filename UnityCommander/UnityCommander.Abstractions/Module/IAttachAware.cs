
namespace UnityCommander.Abstractions.Module
{
    public interface IAttachAware
    {
        void OnAttached(object view);
        void OnDetached();
    }
}
