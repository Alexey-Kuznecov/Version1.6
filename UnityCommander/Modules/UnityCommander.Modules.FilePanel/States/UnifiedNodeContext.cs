
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using UnityCommander.Abstractions.Columns;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Diagnostics.Tracing;
using UnityCommander.Modules.FilePanel.Controllers;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Modules.FilePanel.States.Resolver;
using UnityCommander.Modules.FilePanel.ViewModels;
using UnityCommander.Services.Interfaces;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.States
{
    public sealed class UnifiedNodeContext : BindableBase, IContextMenuHost, IViewportHost, IDisposable
    {
        public FolderNodeContext Folder { get; }
        
        public FileNodeContext File { get; }

        public ICommand OpenCommand { get; }
        
        public ICommand OpenMenuCommand { get; }

        public ObservableCollection<IDirectoryItem> Items { get; } = [];

        public ObservableCollection<MenuItemViewModel> ContextMenuItems { get; }
            = [];

        public ViewportService<IDirectoryItem>? ScrollService { get; private set; }

        public ISelectionManager SelectionManager => File.SelectionManager;

        private IEnumerable<ColumnModel> _columns = [];

        public IEnumerable<ColumnModel> Columns
        {
            get => _columns;
            set => SetProperty(ref _columns, value);
        }

        public ViewportMapper Mapper => File.Mapper;

        public IDropTarget DropTarget => File.DropTarget;

        public Guid TabId => File.TabId;

        public bool IsUnified { get; set; }

        public string Current { get; set; }

        public event Action<
            UnifiedNodeContext,
            IReadOnlyList<IDirectoryItem>>? ItemsAdded;

        public UnifiedNodeContext(
             ContextMenuController menu,
             FolderNodeContext folder,
             FileNodeContext file)
        {
            Folder = folder;
            File = file;

            OpenCommand = new DelegateCommand<IDirectoryItem>(Open);
           
            OpenMenuCommand = new DelegateCommand<object>(parameter =>
            {
                menu.Show(this, parameter);
            });

            ScrollService = new ViewportService<IDirectoryItem>(
                () => Items);

            Folder.Folders.CollectionChanged += OnFoldersChanged;
            File.Files.CollectionChanged += OnFilesChanged;

            RebuildItems();

            Mapper.RangeChanged += OnRangeChanged;
        }

        private void OnFoldersChanged(
            object? sender,
            NotifyCollectionChangedEventArgs e)
        {
            ApplyCollectionChange(e);
        }

        private void OnFilesChanged(
            object? sender,
            NotifyCollectionChangedEventArgs e)
        {
            ApplyCollectionChange(e);
        }

        private void ApplyCollectionChange(
            NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems is null)
                        return;

                    var addedItems =
                        e.NewItems
                            .Cast<IDirectoryItem>()
                            .ToList();

                    foreach (var item in addedItems)
                    {
                        Items.Add(item);
                    }

                    ItemsAdded?.Invoke(
                        this,
                        addedItems);

                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems is not null)
                    {
                        foreach (IDirectoryItem item in e.OldItems)
                            Items.Remove(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems is not null)
                    {
                        for (var i = 0; i < e.NewItems.Count; i++)
                        {
                            if (e.NewItems[i] is IDirectoryItem item &&
                                e.OldItems?[i] is IDirectoryItem oldItem)
                            {
                                var index = Items.IndexOf(oldItem);

                                if (index >= 0)
                                    Items[index] = item;
                            }
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    RebuildItems();
                    break;
            }
        }
        public void RebuildItems()
        {
            Items.Clear();

            foreach (var item in Folder.Folders)
                Items.Add(item);

            foreach (var item in File.Files)
                Items.Add(item);

            _trace.Write(
               "UnifiedNodeContext",
               "ItemsRebuilt",
               new Dictionary<string, object?>
               {
                   ["Count"] = Items.Count
               });

            ItemsAdded?.Invoke(this, Items);
        }

#pragma warning disable CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".
        private IDiagnosticTrace? _trace = ContainerLocator.Container.Resolve<IDiagnosticTrace>();
#pragma warning restore CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".

        private void OnRangeChanged(int start, int end)
        {
            _trace.Write(
                "UnifiedNodeContext",
                "ViewportRangeChanged",
                new Dictionary<string, object?>
                {
                    ["Start"] = start,
                    ["End"] = end,
                    ["ItemCount"] = Items.Count
                });

            ScrollService?.SetRange(start, end);
        }

        private void Open(IDirectoryItem? item)
        {
            if (item is null)
                return;

            if (item is IFolderItem folder)
            {
                Folder.NavigateCommand.Execute(folder);
                return;
            }

            if (item is IFileItem file)
            {
                // File.OpenCommand.Execute(file);
            }
        }

        public void Dispose()
        {
            Mapper.RangeChanged -= ScrollService.SetRange;
            Folder.Folders.CollectionChanged -= OnFoldersChanged;
            File.Files.CollectionChanged -= OnFilesChanged;
            Mapper.RangeChanged -= OnRangeChanged;
        }
    }
}
