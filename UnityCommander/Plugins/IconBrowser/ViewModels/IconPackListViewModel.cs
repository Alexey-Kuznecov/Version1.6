
using IconBrowser.Models;
using IconMaker.Core.Mvvm.Base;
using IconMaker.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace IconBrowser.ViewModels
{
    public sealed class IconPackListViewModel : PropertiesChanged
    {
        private readonly IIconService _service;

        private IconPackInfo? _selectedPack;

        public ObservableCollection<IconPackInfo> Packs { get; }

        public ICommand SelectPackCommand { get; }

        public event Action<string>? PackSelected;

        public IconPackListViewModel(IIconService iconService)
        {
            _service = iconService;

            Packs = new ObservableCollection<IconPackInfo>();

            foreach (var (id, name) in _service.GetPackHeaders())
            {
                Packs.Add(new IconPackInfo
                {
                    Id = id,
                    Name = name
                });
            }
        }

        public IconPackInfo? SelectedPack
        {
            get => _selectedPack;
            set
            {
                _selectedPack = value;
                OnPropertyChanged(nameof(SelectedPack));

                if (value != null)
                {
                    PackSelected?.Invoke(value.Id);
                }
            }
        }
    }
}
