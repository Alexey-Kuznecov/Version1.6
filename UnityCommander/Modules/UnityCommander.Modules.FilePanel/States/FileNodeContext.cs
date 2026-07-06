
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Modules.FilePanel.Controllers;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Services.Interfaces;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.States
{
    public class FileNodeContext : BaseNodeContext, IFileNodeContext, IDisposable
    {
        private ObservableCollection<IFileItem> _files = new();

        public ObservableCollection<IFileItem> Files
        {
            get => _files;
            set => SetProperty(ref _files, value);
        }

        public FileModel SelectedFile { get; set; }

        public string CurrentPath => SelectedFile.Path;

        public ViewportService<IFileItem> ScrollService { get; }

        public FileNodeContext(
             ISelectionManager selection,
             IDropTarget dropTarget,
             ContextMenuController menu,
             ViewportMapper mapper
             ) : base(selection, dropTarget, menu, mapper)
        {
            ScrollService = new ViewportService<IFileItem>(
                   () => Files);

            Mapper.RangeChanged += (start, end) =>
            {
                ScrollService.SetRange(start, end);
            };
        }

        public IFileItem? Find(string path)
        {
            return Files.FirstOrDefault(x => x.Path == path);
        }

        public void Add(IFileItem file)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Files.Add(file);
            });
        }

        public bool Remove(string path)
        {
            var file = Find(path);

            if (file == null)
                return false;

            Application.Current.Dispatcher.Invoke(() =>
            {
                Files.Remove(file);
            });

            return true;
        }

        public bool Update(IFileItem file)
        {
            var current = Find(file.Path);

            if (current == null)
                return false;

            current.Name = file.Name;
            current.Size = file.Size;
            current.Extension = file.Extension;

            return true;
        }

        public void Dispose()
        {
            Mapper.RangeChanged -= ScrollService.SetRange;
        }
    }
}
