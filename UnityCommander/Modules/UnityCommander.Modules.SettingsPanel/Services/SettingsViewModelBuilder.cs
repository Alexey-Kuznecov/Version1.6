
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Modules.SettingsPanel.ViewModels;
using UnityCommander.Settings.Abstactions;

namespace UnityCommander.Modules.SettingsPanel.Services
{
    public class SettingsViewModelBuilder : ISettingsViewModelBuilder
    {
        private readonly ISettingsService _settings;
        private readonly ISettingsEditorFactory _factory;

        public SettingsViewModelBuilder(
            ISettingsService settings,
            ISettingsEditorFactory factory)
        {
            _settings = settings;
            _factory = factory;
        }

        public IEnumerable<SettingsPageViewModel> Build()
        {
            var groups = _settings
                .GetDefinitions()
                .GroupBy(x => x.Category ?? "General");

            foreach (var group in groups)
            {
                var page = new SettingsPageViewModel
                {
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
