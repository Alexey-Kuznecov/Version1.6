using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Modules.SettingsPanel.ViewModels
{
    public class SettingsPanelViewModel : BindableBase
    {
        private string message;
        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }

        public SettingsPanelViewModel()
        {
            Message = "Здесь будет настройки приложения и плагинов";
        }
    }
}
