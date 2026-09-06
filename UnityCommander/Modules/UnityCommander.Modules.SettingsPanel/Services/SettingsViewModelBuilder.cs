
using System.Collections.Generic;
using UnityCommander.Modules.SettingsPanel.ViewModels;

namespace UnityCommander.Modules.SettingsPanel.Services
{
    public class SettingsViewModelBuilder : ISettingsViewModelBuilder
    {
        private readonly IEnumerable<ISettingsSectionProvider> _section;

        public SettingsViewModelBuilder(IEnumerable<ISettingsSectionProvider> section)
        {
            _section = section;
        }

        public IEnumerable<SettingsPageViewModel> Build()
        {
            foreach (var section in _section)
            {
                foreach (var page in section.BuildPages())
                {
                    yield return page;
                }
            }
        }
    }
}
