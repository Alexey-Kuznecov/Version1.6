
using AdvancedCopyFiles.ViewModels;
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Settings;

namespace AdvancedCopyFiles.Core
{
    public interface ICopySettingsBuilder
    {
        CompositeCopySettings Build(
          SettingsViewModel userSettings,
          CopySessionService session);
    }
}
