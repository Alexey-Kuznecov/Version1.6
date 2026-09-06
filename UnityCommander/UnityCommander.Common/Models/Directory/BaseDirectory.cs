
namespace UnityCommander.Common.Models.Directory
{
    using NSwag.Collections;
    using Prism.Mvvm;
    using System;
    using System.Collections.Generic;
    using UnityCommander.Abstractions;
    using UnityCommander.Abstractions.Panels;

    [Serializable]
    public abstract class BaseDirectory : BindableBase, IDirectoryItem
    {
        private bool _isSelected;
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

        public IDictionary<string, object> Additional { get; set; }
            = new ObservableDictionary<string, object>();

        public TargetPanel TargetPanel { get; set; }
        
        public Dictionary<string, DateTime> LastUpdate { get; }
          = new();

        public string Key { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}