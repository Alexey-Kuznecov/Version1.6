
using System;
using System.Collections.ObjectModel;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Core.DragDrop;
using UnityCommander.Modules.FilePanel.Controllers;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.FilePanel.States
{
    public class FileNodeContext : BaseNodeContext, IDisposable
    {
        private ObservableCollection<FileModel> _files = new();

        public ObservableCollection<FileModel> Files
        {
            get => _files;
            set => SetProperty(ref _files, value);
        }

        public FileModel SelectedFile { get; set; }

        public string CurrentPath => SelectedFile.Path;

        public ViewportService<FileModel> ScrollService { get; }

        public FileNodeContext(
             ISelectionManager selection,
             IDropTarget dropTarget,
             ContextMenuController menu,
             ViewportMapper mapper
             ) : base(selection, dropTarget, menu, mapper)
        {
            ScrollService = new ViewportService<FileModel>(
                   () => Files);

            Mapper.RangeChanged += (start, end) =>
            {
                ScrollService.SetRange(start, end);
            };
        }

        public void Dispose()
        {
            Mapper.RangeChanged -= ScrollService.SetRange;
        }
    }
}
