
using Prism.Mvvm;
using System.Collections.ObjectModel;
using UnityCommander.Modules.SettingsPanel.Services;

namespace UnityCommander.Modules.SettingsPanel.ViewModels
{
    public class SettingsPanelViewModel : BindableBase
    {
        public ObservableCollection<SettingsPageViewModel> Pages { get; }

        private SettingsPageViewModel? _selectedPage;

        public SettingsPageViewModel? SelectedPage
        {
            get => _selectedPage;
            set => SetProperty(ref _selectedPage, value);
        }

        public SettingsPanelViewModel(ISettingsViewModelBuilder builder)
        {


            Pages = new ObservableCollection<SettingsPageViewModel>(
                builder.Build());

            SelectedPage = Pages[0];
        }
    }
}
