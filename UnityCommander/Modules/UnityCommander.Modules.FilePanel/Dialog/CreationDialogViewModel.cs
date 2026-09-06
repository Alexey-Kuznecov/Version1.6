
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Modules.FilePanel.Models;

namespace UnityCommander.Modules.FilePanel.Dialog
{
    public sealed class CreationDialogViewModel : BindableBase, IDialogAware<CreationDialogResult>
    {
        private string _name = string.Empty;
        private string _extension = string.Empty;
        private CreationType _type = CreationType.File;
        private string? _templateId;

        private CreationDialogResult? _result;

        public CreationDialogViewModel()
        {
            AcceptCommand = new DelegateCommand(Accept);
            CancelCommand = new DelegateCommand(Cancel);
        }



        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
                AcceptCommand.RaiseCanExecuteChanged();
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

        public CreationType Type
        {
            get => _type;
            set
            {
                if (!SetProperty(ref _type, value))
                    return;

                RaisePropertyChanged(nameof(IsFile));
            }
        }

        public string? TemplateId
        {
            get => _templateId;
            set => SetProperty(ref _templateId, value);
        }

        private CreationExtension? _selectedExtension;

        public CreationExtension? SelectedExtension
        {
            get => _selectedExtension;
            set => SetProperty(ref _selectedExtension, value);
        }

        public bool IsFile =>
            Type == CreationType.File;

        public DelegateCommand AcceptCommand { get; }

        public DelegateCommand CancelCommand { get; }

        public CreationDialogResult Result => _result!;

        public Action? RequestClose { get; set; }

        public bool CanCloseDialog() => true;

        public void OnDialogOpened(object parameter)
        {
            Name = string.Empty;
            Type = CreationType.File;
            TemplateId = null;
            _result = null;
        }

        public void OnDialogClosed()
        {
        }

        private bool CanAccept()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        public void Accept()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return;

            var extension = Type == CreationType.File
                 ? SelectedExtension?.Extension
                 : null;

            _result = new CreationDialogResult()
            {
                Name = Name.Trim(),
                Type = Type,
                Extension = extension,
                TemplateId = TemplateId,
            };

            RequestClose?.Invoke();
        }

        private void Cancel()
        {
            _result = null;
            RequestClose?.Invoke();
        }
    }
}
