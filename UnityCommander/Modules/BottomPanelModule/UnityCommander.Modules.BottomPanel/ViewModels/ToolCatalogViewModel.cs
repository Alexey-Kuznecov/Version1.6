
using System;
using System.Collections.Generic;
using Prism.Commands;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using UnityCommander.Mvvm.Base;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.BottomPanel.ViewModels
{
    public class ToolCatalogViewModel : PropertiesChanged
    {
        private readonly Action<IToolDescriptor> _onSelected;
        private readonly Action _onCanceled;

        private string _searchText = string.Empty;
        private IToolDescriptor? _selectedDescriptor;

        public IReadOnlyCollection<IToolDescriptor> Tools { get; }

        public ICollectionView FilteredTools { get; }

        public IToolDescriptor? SelectedDescriptor
        {
            get => _selectedDescriptor;
            set
            {
                if (_selectedDescriptor == value)
                    return;

                _selectedDescriptor = value;
                OnPropertyChanged(nameof(SelectedDescriptor));

                if (value != null)
                    _onSelected?.Invoke(value);
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value)
                    return;

                _searchText = value;
                OnPropertyChanged(nameof(SearchText));

                FilteredTools.Refresh();
            }
        }

        public DelegateCommand CancelCommand { get; }

        public ToolCatalogViewModel(
            IToolRegistry registry,
            Action<IToolDescriptor> onSelected,
            Action onCanceled)
        {
            _onSelected = onSelected;
            _onCanceled = onCanceled;

            Tools = registry.GetAll().ToList();

            FilteredTools = CollectionViewSource.GetDefaultView(Tools);
            FilteredTools.Filter = FilterTool;

            CancelCommand = new DelegateCommand(
                () => _onCanceled?.Invoke());
        }

        private bool FilterTool(object item)
        {
            if (item is not IToolDescriptor tool)
                return false;

            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            return tool.Title.Contains(
                       SearchText,
                       StringComparison.OrdinalIgnoreCase)
                   || tool.Id.Contains(
                       SearchText,
                       StringComparison.OrdinalIgnoreCase);
        }
    }
}
