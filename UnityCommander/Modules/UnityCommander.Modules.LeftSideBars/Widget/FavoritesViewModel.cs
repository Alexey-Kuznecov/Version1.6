
using CommandSystem.Gui.MVVM;
using Prism.Mvvm;
using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using UnityCommander.Abstractions.History;
using UnityCommander.Common.Panels;
using UnityCommander.Modules.LeftSideBars.Widget.Actions;
using UnityCommander.Modules.LeftSideBars.Widget.Models;

namespace UnityCommander.Modules.LeftSideBars.Widget
{
    public sealed class FavoritesViewModel : BindableBase
    {
        private PanelActionExecutor _executor;

        private FavoriteInfoModel _selectedFolder;

        private readonly IUserFavorites _favorites;

        private readonly ActiveTabContext _activeTabContext;

        public ObservableCollection<FavoriteInfoModel> Favorites { get; } = [];

        public ICommand AddCurrentCommand =>
            new RelayCommand(o => AddCurrent());

        public ICommand ClearCommand => 
            new RelayCommand((o) => Clear());

        public FavoritesViewModel(
            IUserFavorites favorites,
            ActiveTabContext activeTabContext,
            PanelActionExecutor executor)
        {
            _executor = executor;
            _favorites = favorites;
            _activeTabContext = activeTabContext;

            foreach (var path in _favorites.Paths)
                Favorites.Add(CreateInfo(path));
        }

        public FavoriteInfoModel SelectedFavorite
        {
            get => _selectedFolder;
            set
            {
                if (SetProperty(ref _selectedFolder, value))
                {
                    _executor.Navigate(_selectedFolder.Path);
                }
            }
        }

        public void AddCurrent()
        {
            var path = _activeTabContext.CurrentPath;

            if (string.IsNullOrWhiteSpace(path))
                return;

            _favorites.Add(path);

            Favorites.Clear();

            foreach (var item in _favorites.Paths)
                Favorites.Add(CreateInfo(item));
        }

        private static FavoriteInfoModel CreateInfo(string path)
        {
            var fullPath = Path.GetFullPath(path);
            var root = Path.GetPathRoot(fullPath);

            var name = string.Equals(
                fullPath,
                root,
                StringComparison.OrdinalIgnoreCase)
                    ? fullPath
                    : Path.GetFileName(
                        fullPath.TrimEnd(
                            Path.DirectorySeparatorChar,
                            Path.AltDirectorySeparatorChar));

            return new FavoriteInfoModel
            {
                Name = name,
                Path = fullPath,
                IconKey = "folder"
            };
        }

        public void Clear()
            => Favorites.Clear();
    }
}
