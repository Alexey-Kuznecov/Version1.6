
using IconBrowser.Services;
using IconBrowser.Services.Search;
using IconMaker.Core.Models;
using IconMaker.Core.Mvvm.Base;
using IconMaker.Core.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Infrastructure;

namespace IconBrowser.ViewModels
{
    internal class IconMakerViewModel : PropertiesChanged
    {
        private readonly IIconService _iconService;
        private readonly IIconSearchService _searchService;

        private readonly IIconImporter _importer;

        private readonly ILogger _logger;

        private IconPackListViewModel _packListVm;

        private string _currentPackId;

        private IconTheme _currentTheme;

        private IIconThemeService _themeService;

        private string _selectedColor;

        private string _searchQuery;

        private CancellationTokenSource _searchCts;

        public IconPackListViewModel IconPackList => _packListVm;

        public ObservableCollection<IconItemViewModel> Icons { get; } = new();

        public ObservableCollection<IconItemViewModel> SearchResults { get; } = new();

        public IEnumerable<IconItemViewModel> VisibleIcons =>
            IsSearchActive ? SearchResults : Icons;

        public bool IsSearchActive { get; set; }

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

        public IconMakerViewModel(
            IIconService iconService, 
            IIconThemeService themeService,
            IIconSearchService searchService,
            IIconImporter importer,
            LoggerCreator logger)
        {
            _importer = importer;
            _searchService = searchService;
            _logger = logger.For<IconMakerViewModel>();
            _iconService = iconService;
            _themeService = themeService;

            _currentTheme = _themeService.CurrentTheme;
            _currentTheme.PropertyChanged += ThemeChanged;

            _iconService.PackChanged += OnPackChanged;
            _iconService.IconRemoved += OnIconRemoved;
            _iconService.IconUpdated += OnIconUpdated;

            _packListVm = new IconPackListViewModel(_iconService);
            _packListVm.PackSelected += OnPackSelected;
            _themeService.ThemeChanged += OnThemeChanged;

            SelectedColor = _currentTheme.MonochromeColor;

            _ = LoadPack("misk");

            _searchService.RebuildIndexAsync();
        }

        public ICommand SaveCommand => new RelayCommand(obj =>
        {
            _iconService.SaveAll();
        });

        public ICommand ShutdownCommand => new RelayCommand(obj =>
        {
            Application app = Application.Current;
            app.Shutdown();
        });

        public ICommand AddNewIconCommand { get; }

        public ICommand ImportIconCommand => new RelayCommand(obj =>
        {
            ImportSvgs();
        });

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));

                _searchCts?.Cancel();
                _searchCts = new CancellationTokenSource();
                var token = _searchCts.Token;

                _ = RunSearchAsync(_searchQuery, token);
            }
        }

        private async Task RunSearchAsync(string query, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    SearchResults.Clear();
                    IsSearchActive = false;
                    OnPropertyChanged(nameof(VisibleIcons));
                });

                return;
            }

            //await Task.Delay(200, token); // debounce
            
            SearchResults.Clear();

            await _searchService.SearchAsync(
                query,
                new Progress<List<IconSearchResult>>(batch =>
                {
                    foreach (var r in batch)
                    { 
                        SearchResults.Add(CreateVm(r.Definition, r.PackId));
                    }
                }),
                token);

            if (token.IsCancellationRequested)
                return;

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                IsSearchActive = true;
                OnPropertyChanged(nameof(VisibleIcons));
            });
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

        private async Task OnPackSelected(string packId)
        {
            _currentPackId = packId;

            var pack = _iconService.GetPack(packId);

            await LoadIconsAsync(pack, packId);
        }

        private async Task LoadPack(string packId)
        {
            _currentPackId = packId;

            var pack = _iconService.GetPack(packId);

            await LoadIconsAsync(pack, packId);
        }

        public async Task LoadIconsAsync(IconPack pack, string packId)
        {
            Icons.Clear();

            const int batchSize = 100;

            for (int i = 0; i < pack.Icons.Count; i += batchSize)
            {
                var batch = pack.Icons
                    .Skip(i)
                    .Take(batchSize)
                    .Select(icon => CreateVm(icon, packId))
                    .ToList();

                foreach (var vm in batch)
                    Icons.Add(vm);

                await Task.Yield(); // мягче чем Delay(1)
            }
        }

        private IconItemViewModel CreateVm(IconDefinition icon, string packId)
        {
            return new IconItemViewModel(
                icon,
                () => _currentTheme,
                id => _iconService.RemoveIcon(packId, id),
                (id, name) => _iconService.RenameIcon(packId, id, name),
                _logger
            );
        }

        private void OnIconUpdated(string packId, Guid iconId)
        {
            _iconService.SavePack(packId);
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

           _ = LoadPack(packId);
        }

        private void OnIconRemoved(string packId, Guid iconId)
        {
            if (_currentPackId != packId)
                return;

            _ = LoadPack(packId);
        }

        public void ImportSvg()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Import SVG Icon",
                Filter = "SVG files (*.svg)|*.svg",
                Multiselect = false
            };

            if (dialog.ShowDialog() != true)
                return;

            var path = dialog.FileName;

            var name = Path.GetFileNameWithoutExtension(path);

            var definition = _importer.Import(path, name);

            var iconItem = CreateVm(definition, _currentPackId);

            Icons.Add(iconItem);
        }

        public void ImportSvgs()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Import SVG Icons",
                Filter = "SVG files (*.svg)|*.svg",
                Multiselect = true
            };

            if (dialog.ShowDialog() != true)
                return;

            foreach (var path in dialog.FileNames)
            {
                var name = Path.GetFileNameWithoutExtension(path);

                var definition = _importer.Import(path, name);

                var iconItem = CreateVm(definition, _currentPackId);

                Icons.Add(iconItem);
                _iconService.GetPack("misk").Icons.Add(definition);
            }
        }
    }
}
