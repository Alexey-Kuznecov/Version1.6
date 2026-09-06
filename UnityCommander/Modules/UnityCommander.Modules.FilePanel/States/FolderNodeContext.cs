
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Core.Navigation;
using UnityCommander.Logging;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Modules.FilePanel.Controllers;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Services.Interfaces;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.States
{
    public class FolderNodeContext : BaseNodeContext, IDisposable, IFolderNodeContext
    {
        private ObservableCollection<IFolderItem> _folders = new ();
        
        public ObservableCollection<IFolderItem> Folders
        {
            get => _folders;
            set => SetProperty(ref _folders, value);
        }

        public FolderModel SelectedFolder { get; set; }

        public ICommand NavigateCommand { get; set; }

        public ViewportService<IFolderItem>? ScrollService { get; }

        private readonly ILogger _logger;

        public FolderNodeContext(
           ISelectionManager selection,
           IDropTarget dropTarget,
           ContextMenuController menu,
           NavigationManager navigation,
           ViewportMapper mapper
            ) : base(selection, dropTarget, menu, mapper)
        {
            _logger = Log.Create("Navigation", LogScope.UserAction);

            ScrollService = new ViewportService<IFolderItem>(
                    () => Folders);

            NavigateCommand = new DelegateCommand<IFolderItem>(dir =>
            {
                var sw = Stopwatch.StartNew();

                if (dir != null)
                    navigation.TryNavigateTo(dir.Path);
                sw.Stop();
//#if (Nlog)
//                _logger.Info($"Переход в папку ({dir.Path}) заняло: {sw.ElapsedMilliseconds} ms, всего папок: {Folders.Count}");
//#endif
            });

            Mapper.RangeChanged += (start, end) =>
            {
                ScrollService.SetRange(start, end);
            };
        }

        public IFolderItem Find(string path)
        {
            return Folders.FirstOrDefault(x => x.Path == path);
        }

        public void Add(IFolderItem folder)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Folders.Add(folder);
            });
        }

        public bool Remove(string path)
        {
            var folder = Find(path);

            if (folder == null)
                return false;

            Application.Current.Dispatcher.Invoke(() =>
            {
                Folders.Remove(folder);
            });

            return true;
        }

        public bool Update(IFolderItem folder)
        {
            var current = Find(folder.Path);

            if (current == null)
                return false;

            current.Name = folder.Name;

            return true;
        }

        public bool Rename(string oldPath, string newPath)
        {
            string? newName = Path.GetFileName(newPath);

            var current = Find(oldPath);

            if (current == null)
                return false;

            current.Name = newName;
            current.Path = newPath;

            return true;
        }

        public void Dispose()
        {
            Mapper.RangeChanged -= ScrollService.SetRange;
        }
    }
}
