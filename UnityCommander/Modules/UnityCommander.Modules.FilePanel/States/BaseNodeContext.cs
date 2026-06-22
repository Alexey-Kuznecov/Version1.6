
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using UnityCommander.Abstractions.Columns;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Core.Helper;
using UnityCommander.Modules.FilePanel.Controllers;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Modules.FilePanel.States.Resolver;
using UnityCommander.Modules.FilePanel.ViewModels;
using UnityCommander.Services.Interfaces;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.States
{
    public abstract class BaseNodeContext : BindableBase, IContextMenuHost, IViewportHost
    {
        public string _current;
        public IEnumerable<ColumnModel> _columns = new List<ColumnModel>();
        public ObservableCollection<MenuItemViewModel> _context = new();
        public ObservableCollection<BaseDirectory> _selected = new();

        protected BaseNodeContext(
           ISelectionManager selection,
           IDropTarget dropTarget,
           ContextMenuController menu, 
           ViewportMapper mapper)
        {
            Mapper = new ViewportMapper();
            SelectionManager = selection;
            DropTarget = dropTarget;
            ShowContextMenuCommand = new DelegateCommand<object>(x =>
            {
                menu.Show(this, x);
            });
        }

        public ViewportMapper Mapper { get; }

        public ICommand ShowContextMenuCommand { get; set; }

        public ISelectionManager SelectionManager { get; set; }

        public string Current
        {
            get => _current;
            set => SetProperty(ref _current, value);
        }

        public IEnumerable<ColumnModel> Columns
        {
            get => _columns;
            set => SetProperty(ref _columns, value);
        }
     
        public ObservableCollection<MenuItemViewModel> ContextMenuItems
        {
            get => _context;
            set => SetProperty(ref _context, value);
        }

        public ObservableCollection<BaseDirectory> SelectedItems 
            => SelectionManager.SelectedItems
                    .OfType<BaseDirectory>()
                    .Where(x => x.IsSelected)
                    .ToObservableCollection();

        public IDropTarget DropTarget
        {
            get;
            init;
        }
    }
}
