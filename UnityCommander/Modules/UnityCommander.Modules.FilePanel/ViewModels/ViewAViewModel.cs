
namespace UnityCommander.Modules.FilePanel.ViewModels
{
    using Prism.Regions;
    using UnityCommander.Core.Mvvm;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The view a view model.
    /// </summary>
    public class ViewAViewModel : RegionViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAViewModel"/> class.
        /// </summary>
        /// <param name="regionManager">
        /// The region manager.
        /// </param>
        /// <param name="directoryProvider">
        /// The file provider.
        /// </param>
        public ViewAViewModel(IRegionManager regionManager, IDirectoryProvider directoryProvider)
            : base(regionManager)
        {
        }

        /// <summary>
        /// The on navigated to.
        /// </summary>
        /// <param name="navigationContext">
        /// The navigation context.
        /// </param>
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            // do something
        }
    }
}
