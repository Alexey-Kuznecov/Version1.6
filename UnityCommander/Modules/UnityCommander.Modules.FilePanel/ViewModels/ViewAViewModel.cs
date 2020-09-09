
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
        /// The _file list.
        /// </summary>
        private ObservableCollection<FileModel> fileList;

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
        /// Gets or sets the file list.
        /// </summary>
        public ObservableCollection<FileModel> FileList
        {
            get => this.fileList;
            set => this.SetProperty(ref this.fileList, value);
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
