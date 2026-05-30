

using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Modules.FilePanel.States.Resolver;
using UnityCommander.Modules.FilePanel.ViewModels;

namespace UnityCommander.Modules.FilePanel.States
{
    public class DriveNodeContext : BindableBase, IContextMenuHost
    {
        private ObservableCollection<DriveModel> _drives = new();

        public ObservableCollection<DriveModel> Drives
        {
            get => _drives;
            set => SetProperty(ref _drives, value);
        }

        public IEnumerable<ColumnModel> _columns = new List<ColumnModel>();

        public IEnumerable<ColumnModel> Columns
        {
            get => _columns;
            set => SetProperty(ref _columns, value);
        }

        public ICommand ShowContextMenuCommand { get; set; }
       
        public ICommand NavigateCommand { get; set; }

        public DriveModel Selected { get; set; }

        public ObservableCollection<MenuItemViewModel> ContextMenuItems { get; }
    }
}
