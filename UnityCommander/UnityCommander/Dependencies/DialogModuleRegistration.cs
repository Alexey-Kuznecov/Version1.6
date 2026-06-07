
using Prism.Dialogs;
using Prism.Ioc;
using UnityCommander.Bootstrap;
using UnityCommander.Modules.Viewer.Views;
using UnityCommander.ViewModels;
using UnityCommander.ViewModels.Dialogs;
using UnityCommander.Views;
using UnityCommander.Views.CopyDialogs;

namespace UnityCommander.Dependencies
{
    public static class DialogModuleRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            // -------------------------------
            // 2. Регистрация диалогов
            // -------------------------------
            // Каждый диалог регистрируется с View и ViewModel
            registry.RegisterDialog<DialogView, DialogViewModel>("DialogPlugin");
            registry.RegisterDialog<CopyDialogView, CopyDialogViewModel>("CopyDialog");
            registry.RegisterDialog<CopyDialogSkipReplace, CopyDialogSkipReplaceViewModel>("CopyDialogSkipReplace");
            registry.RegisterDialog<AppConfigDialogControl, AppConfigDialogViewModel>("AppConfigDialog");

            //registry.RegisterDialog<DialogPluginConfigView, DialogPluginConfigVm>("DialogPluginConfig"); // пока закомментирован
            
            registry.RegisterSingleton<IDialogService, OverrideDialogService>();
        }
    }
}
