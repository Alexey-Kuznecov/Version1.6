
namespace UnityCommander.Core.Mvvm
{
    using System;
    using Prism.Navigation.Regions;

    public class RegionViewModelBase : ViewModelBase, INavigationAware, IConfirmNavigationRequest
    {
        public RegionViewModelBase(IRegionManager regionManager)
        {
            this.RegionManager = regionManager;
        }

        protected IRegionManager RegionManager { get; }

        public virtual void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            continuationCallback(true);
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }
    }
}
