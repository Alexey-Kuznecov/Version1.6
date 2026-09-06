
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Modules.SettingsPanel.ViewModels;
using UnityCommander.Settings.Abstactions;

namespace UnityCommander.Modules.SettingsPanel.Services
{
    public class SettingsSectionProvider : ISettingsSectionProvider
    {
        private readonly ISettingsService _settings;
        private readonly ISettingsEditorFactory _factory;

        public SettingsSectionProvider(
            ISettingsService settings,
            ISettingsEditorFactory factory)
        {
            _settings = settings;
            _factory = factory;
        }

        public IEnumerable<SettingsPageViewModel> BuildPages()
        {
            var groups = _settings
                .GetDefinitions()
                .GroupBy(x => x.Category ?? "General");

            foreach (var group in groups)
            {
                var page = new SettingsPageViewModel
                {
                    IconKey = group.Key,
                    Title = group.Key
                };

                foreach (var def in group)
                {
                    var value = _settings.Get(def);

                    var item = _factory.Create(def, value);

                    page.Items.Add(item);
                }

                yield return page;
            }
        }
    }
}
