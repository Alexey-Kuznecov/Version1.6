
using IconMaker.Core.Models;
using IconMaker.Core.Mvvm.Base;
using IconMaker.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace IconBrowser.ViewModels
{
    internal class IconMakerViewModel : PropertiesChanged
    {
        private readonly IIconService _iconService;

        private IconPackListViewModel _packListVm;

        private string _currentPackId;

        private IconTheme _currentTheme;

        private IIconThemeService _themeService;

        private string _selectedColor;

        public IconPackListViewModel IconPackList => _packListVm;

        public ObservableCollection<IconItemViewModel> Icons { get; }
            = new();

        public ObservableCollection<string> AvailableColors { get; } =
            new()
            {
                "#FF3676AE",
                "#ee2c2c00",
                "#FF79ED9A",
                "#FF184B9C",
                "#FFE1D854",
                "#FF9b993a",
                "#FFbb4b9c",
                "#FFF17A07",
                "#FF78c696",
                "#FFf59eb3",
                "#FFc5a3cc",
                "#FFcec82b",
                "#FFe0b03a",
                "#FF8d663a",
                "#FF028d5b",
                "#FF009FE3",
                "#FF672f8f"
            };

        public IconMakerViewModel(IIconService iconService, IIconThemeService themeService)
        {
            _iconService = iconService;
            _themeService = themeService;

            _currentTheme = _themeService.CurrentTheme;
            _currentTheme.PropertyChanged += ThemeChanged;

            _iconService.PackChanged += OnPackChanged;
            _iconService.IconRemoved += OnIconRemoved;

            _packListVm = new IconPackListViewModel(_iconService);
            _packListVm.PackSelected += OnPackSelected;
            _themeService.ThemeChanged += OnThemeChanged;

            SelectedColor = _currentTheme.MonochromeColor;
            LoadPack("misk");
        }

        private void ThemeChanged(object sender, PropertyChangedEventArgs e)
        {
            foreach (var icon in Icons)
            {
                icon.Refresh();
            }
        }

        public IconTheme CurrentTheme
        {
            get => _currentTheme;
            set
            {
                _currentTheme = value;
                OnPropertyChanged(nameof(CurrentTheme));

                if (value != null)
                {
                  
                }
            }
        }

        public string SelectedColor
        {
            get => _selectedColor;
            set
            {
                _selectedColor = value;
                OnPropertyChanged(nameof(SelectedColor));

                _currentTheme.MonochromeColor = value;
            }
        }

        private void OnPackSelected(string packId)
        {
            _currentPackId = packId;

            var pack = _iconService.GetPack(packId);

            Icons.Clear();

            foreach (var icon in pack.Icons)
            {
                Icons.Add(CreateVm(icon, packId));
            }
        }

        private void LoadPack(string packId)
        {
            _currentPackId = packId;

            var pack = _iconService.GetPack(packId);

            Icons.Clear();

            foreach (var icon in pack.Icons)
            {
                Icons.Add(CreateVm(icon, packId));
            }
        }

        private IconItemViewModel CreateVm(IconDefinition icon, string packId)
        {
            return new IconItemViewModel(
                icon,
                () => _currentTheme,
                id => _iconService.RemoveIcon(packId, id),
                (id, name) => _iconService.RenameIcon(packId, id, name)
            );
        }

        private void OnThemeChanged(string id)
        {
            foreach (var icon in Icons)
            {
                icon.Refresh();
            }
        }

        private void OnPackChanged(string packId)
        {
            if (_currentPackId != packId)
                return;

            LoadPack(packId);
        }

        private void OnIconRemoved(string packId, Guid iconId)
        {
            if (_currentPackId != packId)
                return;

            LoadPack(packId);
        }
    }
}
