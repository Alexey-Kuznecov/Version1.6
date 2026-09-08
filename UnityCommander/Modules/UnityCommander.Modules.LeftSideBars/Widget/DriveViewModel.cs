
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UnityCommander.Modules.LeftSideBars.Widget.Models;
using UnityCommander.Rendering.Icons;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.LeftSideBars.Widget
{
    public class DriveViewModel
    {
        public string Path { get; }

        public ObservableCollection<DriveInfoModel> Drives { get; } = [];

        public DriveViewModel(
            IDataProviderService dataProvider, 
            IIconRenderService iconResolver)
        {
            _ = GoDrivePanel(dataProvider, iconResolver);
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
