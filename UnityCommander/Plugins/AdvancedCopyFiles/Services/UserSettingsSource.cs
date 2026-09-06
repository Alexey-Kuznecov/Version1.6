
using AdvancedCopyFiles.ViewModels;
using UnityCommander.Copying.Settings;

namespace AdvancedCopyFiles.Services
{
    public class UserSettingsSource : ICopySetting
    {
        private readonly SettingsViewModel _vm;

        public UserSettingsSource(SettingsViewModel vm)
        {
            _vm = vm;
        }

        public void Apply(ref CopyOptions options)
        {
            options.MaxConсurrentTasks = _vm.MaxConcurrentTasks;
            options.UseMultiThreading = _vm.UseMultiThreading;
            options.OverwriteExistingFiles = _vm.OverwriteExistingFiles;
        }
    }
}
