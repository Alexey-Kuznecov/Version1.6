using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityCommander.Business;
using UnityCommander.Core.Mvvm;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    public class ViewAViewModel : RegionViewModelBase
    {
        private ObservableCollection<FileModel> _fileList;

        public ObservableCollection<FileModel> FileList
        {
            get { return _fileList; }
            set { SetProperty(ref _fileList, value); }
        }

        public ViewAViewModel(IRegionManager regionManager, IFilesProvider fileProvider)
            : base(regionManager)
        {
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            //do something
        }
    }
}
