
namespace UnityCommander.Common.Models.Directory
{
    using Prism.Mvvm;
    using System;
    using System.Collections.Generic;
    using UnityCommander.Abstractions;
    using UnityCommander.Abstractions.Panels;
    using UnityCommander.Abstractions.Thumbnail;

    [Serializable]
    public abstract class BaseDirectory : BindableBase, IDirectoryItem
    {
        private string _name;
        private string _path;
        private string _iconKey;
        private IconKind _kind;
        private DateTime _creationTime;
        private DateTime _lastAccessTime;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Path
        {
            get => _path;
            set => SetProperty(ref _path, value);
        }

        public string IconKey
        {
            get => _iconKey;
            set => SetProperty(ref _iconKey, value);
        }

        private ThumbnailResult? _thumbnail;

        public ThumbnailResult? Thumbnail
        {
            get => _thumbnail;
            set => SetProperty(ref _thumbnail, value);
        }

        public IconKind Kind
        {
            get => _kind;
            set => SetProperty(ref _kind, value);
        }

        public DateTime CreationTime
        {
            get => _creationTime;
            set => SetProperty(ref _creationTime, value);
        }

        public DateTime LastAccessTime
        {
            get => _lastAccessTime;
            set => SetProperty(ref _lastAccessTime, value);
        }

        //public IDictionary<string, object> Additional { get; set; }
        //    = new ObservableDictionary<string, object>();

        public TargetPanel TargetPanel { get; set; }
        
        public Dictionary<string, DateTime> LastUpdate { get; }
          = new();

        public string Key { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private bool _isCut;

        public bool IsCut
        {
            get => _isCut;
            set => SetProperty(ref _isCut, value);
        }

        private ICellModel _cell = new CellModel();

        public ICellModel Cell
        {
            get => _cell;
            set => SetProperty(ref _cell, value);
        }
    }
}