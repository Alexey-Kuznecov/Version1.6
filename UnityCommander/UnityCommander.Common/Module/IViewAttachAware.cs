
namespace UnityCommander.Common.Module
{
    public interface IViewAttachAware
    {
        void OnViewAttached(object view);
        void OnViewDetached();
    }
}
