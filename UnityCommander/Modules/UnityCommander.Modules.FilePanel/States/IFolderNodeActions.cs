
using UnityCommander.Common.Models.Directory;

namespace UnityCommander.Modules.FilePanel.States
{
    public interface IFolderNodeActions
    {
        void Navigate(FolderModel model);
        void ShowContextMenu(object parameter);
    }
}
