
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Modules.FilePanel.States.Resolver;
using UnityCommander.Modules.FilePanel.ViewModels;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.FilePanel.States
{
    public abstract class BaseNodeContext : BindableBase, IContextMenuHost
    {
        public string _current;

        public string Current
        {
            get => _current;
            set => SetProperty(ref _current, value);
        }

        public IEnumerable<ColumnModel> _columns = new List<ColumnModel>();

        public IEnumerable<ColumnModel> Columns
        {
            get => _columns;
            set => SetProperty(ref _columns, value);
        }

        public ISelectionManager SelectionManager { get; set; }

        public ObservableCollection<MenuItemViewModel> _context = new();

        public ObservableCollection<MenuItemViewModel> ContextMenuItems
        {
            get => _context;
            set => SetProperty(ref _context, value);
        }

        public ObservableCollection<BaseDirectory> _selected = new();

        public ObservableCollection<BaseDirectory> SelectedItems
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public ICommand ShowContextMenuCommand { get; set; }
    }
}
