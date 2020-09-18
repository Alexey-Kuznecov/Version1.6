
namespace UnityCommander.Modules.FilePanel.ViewModels
{
    using System.Collections.ObjectModel;

    using Prism.Regions;

    using UnityCommander.Business;
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
        /// <param name="fileProvider">
        /// The file provider.
        /// </param>
        public ViewAViewModel(IRegionManager regionManager, IFilesProvider fileProvider)
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
