
namespace UnityCommander.Core.Mvvm
{
    using System;
    using Prism.Navigation.Regions;

    /// <summary>
    /// The region view model base.
    /// </summary>
    public class RegionViewModelBase : ViewModelBase, INavigationAware, IConfirmNavigationRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionViewModelBase"/> class.
        /// </summary>
        /// <param name="regionManager">
        /// The region manager.
        /// </param>
        public RegionViewModelBase(IRegionManager regionManager)
        {
            this.RegionManager = regionManager;
        }

        /// <summary>
        /// Gets the region manager.
        /// </summary>
        protected IRegionManager RegionManager { get; }

        /// <summary>
        /// The confirm navigation request.
        /// </summary>
        /// <param name="navigationContext">
        /// The navigation context.
        /// </param>
        /// <param name="continuationCallback">
        /// The continuation callback.
        /// </param>
        public virtual void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            continuationCallback(true);
        }

        /// <summary>
        /// The is navigation target.
        /// </summary>
        /// <param name="navigationContext">
        /// The navigation context.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        /// <summary>
        /// The on navigated from.
        /// </summary>
        /// <param name="navigationContext">
        /// The navigation context.
        /// </param>
        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        /// <summary>
        /// The on navigated to.
        /// </summary>
        /// <param name="navigationContext">
        /// The navigation context.
        /// </param>
        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }
    }
}
