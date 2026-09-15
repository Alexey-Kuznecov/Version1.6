
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using UnityCommander.Abstractions.Columns;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Services.Interfaces;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.States
{
    public sealed class UnifiedNodeContext : BindableBase, IViewportHost, IDisposable
    {
        public FolderNodeContext Folder { get; }
        public FileNodeContext File { get; }

        public ICommand OpenCommand { get; }

        public ObservableCollection<IDirectoryItem> Items { get; } = [];

        public ViewportMapper Mapper { get; }

        public ViewportService<IDirectoryItem>? ScrollService { get; }

        public ISelectionManager SelectionManager => File.SelectionManager;

        private IEnumerable<ColumnModel> _columns = [];

        public IEnumerable<ColumnModel> Columns
        {
            get => _columns;
            set => SetProperty(ref _columns, value);
        }

        public IDropTarget DropTarget => File.DropTarget;

        public UnifiedNodeContext(
            FolderNodeContext folder,
            FileNodeContext file)
        {
            Folder = folder;
            File = file;

            //ScrollService = new ViewportService<IDirectoryItem>(
            //    () => Items);

            Mapper = new ViewportMapper();

            OpenCommand = new DelegateCommand<IDirectoryItem>(Open);

            Folder.Folders.CollectionChanged += OnFoldersChanged;
            File.Files.CollectionChanged += OnFilesChanged;

            RebuildItems();

            //Mapper.RangeChanged += OnRangeChanged;
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
                    if (e.NewItems is not null)
                    {
                        foreach (IDirectoryItem item in e.NewItems)
                            Items.Add(item);
                    }
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
        }

        private void OnRangeChanged(int start, int end)
        {
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
            Folder.Folders.CollectionChanged -= OnFoldersChanged;
            File.Files.CollectionChanged -= OnFilesChanged;
            Mapper.RangeChanged -= OnRangeChanged;
        }
    }
}
