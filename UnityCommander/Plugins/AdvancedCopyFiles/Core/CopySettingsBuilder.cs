
using AdvancedCopyFiles.ViewModels;
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Settings;

namespace AdvancedCopyFiles.Core
{
    public class CopySettingsBuilder : ICopySettingsBuilder
    {
        public CompositeCopySettings Build(
            SettingsViewModel userSettings,
            CopySessionService session)
        {
            var composite = new CompositeCopySettings();
            // дефолтные
            composite.Add(SettingPriority.Default, opts =>
            {
                opts.UseMultiThreading = true;
                opts.MaxConсurrentTasks = 5;
                opts.UseCategories = true;
                opts.UseMetrics = true;
                opts.UseDualChannels = false;
                opts.UseParallel = true;
                // Новое
                opts.BufferSize = 64 * 1024;
                opts.MinBufferSize = 8 * 1024;
                opts.VerboseLogging = true;
            });

            // сессионные
            composite.Add(SettingPriority.Session, opts =>
            {
                session.CurrentSession.ProgressStep = 10;
                session.CurrentSession.VerboseLogging = true;
                opts.VerboseLogging = session.CurrentSession.VerboseLogging;
            });

            // пользовательские (имеют больший приоритет)
            composite.Add(SettingPriority.User, opts =>
            {
                opts.MaxConсurrentTasks = userSettings.MaxConcurrentTasks; // переопределение
                opts.UseMultiThreading = userSettings.UseMultiThreading;
                opts.UseParallel = userSettings.UseMultiThreading;
                opts.UseProgressiveDiscovery = false;
                opts.UseWinApi = false;
            });

            return composite;
        }
    }
}
