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
        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public SettingsPanelViewModel()
        {
            Message = "View A from your Prism Module";
        }
    }
}
