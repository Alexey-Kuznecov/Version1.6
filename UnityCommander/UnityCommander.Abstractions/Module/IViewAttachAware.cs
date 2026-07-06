namespace UnityCommander.Abstractions.Module
{
    public interface IViewAttachAware
    {
        void OnViewAttached(object view);
        void OnViewDetached();
    }
}
