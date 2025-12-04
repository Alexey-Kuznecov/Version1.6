
namespace UnityCommander.Core.Mvvm
{
    using Prism.Mvvm;
    using Prism.Navigation;

    /// <summary>
    /// The view model base.
    /// </summary>
    public abstract class ViewModelBase : BindableBase, IDestructible
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        protected ViewModelBase()
        {
        }

        /// <summary>
        /// The destroy.
        /// </summary>
        public virtual void Destroy()
        {
        }
    }
}
