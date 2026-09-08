
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using UnityCommander.Common.Panels;
using UnityCommander.Modules.FilePanel.Models;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.WPF;

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    public sealed class CreateFileViewModel : BindableBase
    {
        private readonly ActiveTabContext _activeTab;
        private readonly ICreationService _creationService;
        private readonly IPopupService _popupService;
        private string _name = string.Empty;
        private CreationExtension? _selectedExtension;
        private string? _templateId;

        public CreateFileViewModel(
            ICreationService creationService,
            IPopupService popupService,
            ActiveTabContext activeTab)
        {
            _popupService = popupService;
            _activeTab = activeTab;
            _creationService = creationService;
            CreateCommand = new DelegateCommand(Create, CanCreate);
            CreateAndOpenCommand = new DelegateCommand(CreateAndOpen, CanCreate);
            SelectedExtension = Extensions[0];
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

        public IReadOnlyList<CreationExtension> Extensions { get; } =
        [
            new(".txt", "Текстовый файл (.txt)"),
            new(".json", "JSON (.json)"),
            new(".xml", "XML (.xml)"),
            new(".cs", "C# (.cs)"),
            new(".xaml", "XAML (.xaml)"),
            new(".md", "Markdown (.md)")
        ];

        public CreationExtension? SelectedExtension
        {
            get => _selectedExtension;
            set
            {
                if (!SetProperty(ref _selectedExtension, value))
                    return;

                CreateCommand.RaiseCanExecuteChanged();
                CreateAndOpenCommand.RaiseCanExecuteChanged();
            }
        }

        public string? TemplateId
        {
            get => _templateId;
            set => SetProperty(ref _templateId, value);
        }

        public DelegateCommand CreateCommand { get; }

        public DelegateCommand CreateAndOpenCommand { get; }

        private bool CanCreate()
        {
            return !string.IsNullOrWhiteSpace(Name)
                && SelectedExtension is not null;
        }

        private void Create()
        {
            var path = _activeTab.CurrentPath;

            var extension = SelectedExtension?.Extension;

            var creation = new CreationContext(Name, path, extension, CreationType.File, null);

            _creationService.CreateAsync(creation);

            _popupService.Close();
        }

        private void CreateAndOpen()
        {
            // создание файла + переход
        }
    }
}
