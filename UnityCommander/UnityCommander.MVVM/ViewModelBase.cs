
namespace UnityCommander.Core.Mvvm
{
    using Prism.Mvvm;
    using Prism.Navigation;

    /// <summary>
    /// The view model base.
    /// </summary>
    public abstract class ViewModelBase : BindableBase, IDestructible
    {
        protected ViewModelBase()
        {
        }

        public virtual void Destroy()
        {
        }
    }
}
