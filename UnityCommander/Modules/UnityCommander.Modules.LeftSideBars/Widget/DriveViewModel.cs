
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UnityCommander.Modules.LeftSideBars.Widget.Actions;
using UnityCommander.Modules.LeftSideBars.Widget.Models;
using UnityCommander.Rendering.Icons;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.LeftSideBars.Widget
{
    public class DriveViewModel : BindableBase
    {
        private PanelActionExecutor _executor;

        public string Path { get; }

        public ObservableCollection<DriveInfoModel> Drives { get; } = [];

        public DriveViewModel(
            IDataProviderService dataProvider, 
            IIconRenderService iconResolver, 
            PanelActionExecutor executor)
        {
            _executor = executor;
            _ = GoDrivePanel(dataProvider, iconResolver);
        }

        private DriveInfoModel _selectedDrive;

        public DriveInfoModel SelectedDrive
        {
            get => _selectedDrive;
            set
            {
                if (SetProperty(ref _selectedDrive, value))
                {
                    _executor.Navigate(_selectedDrive.Letter);
                }
            }
        }

        private async Task GoDrivePanel(
            IDataProviderService dataProvider, 
            IIconRenderService iconResolver)
        {
            var drives = await dataProvider.GetDrivesAsync();
         
            foreach (var d in drives)
            {
                Drives.Add(new DriveInfoModel
                {
                    Letter = d.Letter,
                    //Name = d.Name,
                    IconKey = "hard-drive",
                    TotalSize = d.TotalAmount,
                    AvailableFreeSpace = d.FreeSpace
                });

            }
        }
    }
}
