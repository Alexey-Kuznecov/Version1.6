
namespace UnityCommander.Common.Module
{
    public interface IAttachAware
    {
        void OnAttached(object view);
        void OnDetached();
    }
}
