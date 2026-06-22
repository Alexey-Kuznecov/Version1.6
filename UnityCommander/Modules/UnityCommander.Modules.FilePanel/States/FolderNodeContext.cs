
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using UnityCommander.Common.Models.Directory;
using UnityCommander.WPF.DragDrop;
using UnityCommander.Core.Navigation;
using UnityCommander.Modules.FilePanel.Controllers;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Services.Interfaces;
using static UnityCommander.Common.Commands.CommandNames;

namespace UnityCommander.Modules.FilePanel.States
{
    public class FolderNodeContext : BaseNodeContext, IDisposable
    {
        private ObservableCollection<FolderModel> _folders = new ();
        
        public ObservableCollection<FolderModel> Folders
        {
            get => _folders;
            set => SetProperty(ref _folders, value);
        }

        public FolderModel SelectedFolder { get; set; }

        public ICommand NavigateCommand { get; set; }


        public ViewportService<FolderModel>? ScrollService { get; }

        public FolderNodeContext(
           ISelectionManager selection,
           IDropTarget dropTarget,
           ContextMenuController menu,
           NavigationManager navigation,
           ViewportMapper mapper
            ) : base(selection, dropTarget, menu, mapper)
        {
            ScrollService = new ViewportService<FolderModel>(
                    () => Folders);

            NavigateCommand = new DelegateCommand<FolderModel>(dir =>
            {
                var sw = Stopwatch.StartNew();

                if (dir != null)
                    navigation.TryNavigateTo(dir.Path);
                sw.Stop();

                Debug.WriteLine($"NavigateTo: {sw.ElapsedMilliseconds} ms");
            });

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
