
using System.Collections.Generic;
using UnityCommander.Modules.SettingsPanel.ViewModels;

namespace UnityCommander.Modules.SettingsPanel.Services
{
    public interface ISettingsViewModelBuilder
    {
        IEnumerable<SettingsPageViewModel> Build();
    }
}