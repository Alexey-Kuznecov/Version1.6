
using IconBrowser.Models;
using IconMaker.Core.ImportExport;
using IconMaker.Core.Models;
using IconMaker.Core.Services;
using IconMaker.Core.Storage;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace IconBrowser.ViewModels
{
    internal class IconMakerViewModel
    {
        private readonly IIconService _iconService;

        private IconPackListViewModel _packListVm;
        private string _currentPackId;

        public IconPackListViewModel IconPackList => _packListVm;

        public ObservableCollection<IconItemViewModel> Icons { get; }
            = new();

        public IconMakerViewModel()
        {
            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "plugins",
                "IconBrowser",
                "Data");

            var fileSystem = new FileSystem();

            var serializer = new JsonIconSerializer();

            var storage = new JsonIconStorage(
                path,
                fileSystem,
                serializer);

            var store = new IconStore(storage);

            _iconService = new IconService(store);
            _iconService.PackChanged += OnPackChanged;
            _iconService.IconRemoved += OnIconRemoved;


            _packListVm = new IconPackListViewModel(_iconService);
            _packListVm.PackSelected += OnPackSelected;

            var reader = new XmlIconReader();
            var pathData = Directory.GetCurrentDirectory() + @"\plugins\IconBrowser\Data\IconsData.xml";

            //var id = "misk";
            //var name = "Разные";

            //var icons = reader.Read(pathData, name);

            //var pack = new IconPack(
            //    id,
            //    name,
            //    icons);

            //_iconService.ImportPack(pack);
            //_iconService.SavePack(pack.Id);
            LoadPack("misk");
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
                id => _iconService.RemoveIcon(packId, id),
                (id, name) => _iconService.RenameIcon(packId, id, name)
            );
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
