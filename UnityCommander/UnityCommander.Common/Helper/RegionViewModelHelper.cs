
using Prism.Navigation.Regions;
using System.Linq;
using System.Windows;

namespace UnityCommander.Common
{
    public static class RegionViewModelHelper
    {
        public static TViewModel? GetViewModel<TViewModel>(
            IRegionManager regionManager,
            string regionName)
            where TViewModel : class
        {
            var view = regionManager
                .Regions[regionName]
                .ActiveViews
                .FirstOrDefault() as FrameworkElement;

            return view?.DataContext as TViewModel;
        }
    }
}
