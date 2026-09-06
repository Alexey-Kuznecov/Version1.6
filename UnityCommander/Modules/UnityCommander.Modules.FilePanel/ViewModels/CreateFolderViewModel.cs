
using Prism.Commands;
using Prism.Mvvm;
using UnityCommander.Common.Panels;
using UnityCommander.Modules.FilePanel.Models;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.WPF;

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    public sealed class CreateFolderViewModel : BindableBase
    {
        private readonly ActiveTab _activeTab;
        private readonly IPopupService _popupService;
        private readonly ICreationService _creationService;

        private string _name = string.Empty;

        public CreateFolderViewModel(
             IPopupService popupService,
             ICreationService creationService,
             ActiveTab activeTab)
        {
            _popupService = popupService;
            _activeTab = activeTab;
            _creationService = creationService;
            CreateCommand = new DelegateCommand(Create, CanCreate);
            CreateAndOpenCommand = new DelegateCommand(CreateAndOpen, CanCreate);
        }

        public string Name
        {
            get => _name;
            set
            {
                if (!SetProperty(ref _name, value))
                    return;

                CreateCommand.RaiseCanExecuteChanged();
                CreateAndOpenCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand CreateCommand { get; }

        public DelegateCommand CreateAndOpenCommand { get; }

        private bool CanCreate()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        private void Create()
        {
            var path = _activeTab.CurrentPath;

            var creation = new CreationContext(Name, path, null, CreationType.Directory, null);

            _creationService.CreateAsync(creation);

            _popupService.Close();
        }

        private void CreateAndOpen()
        {
            // создание папки + переход
        }
    }
}
