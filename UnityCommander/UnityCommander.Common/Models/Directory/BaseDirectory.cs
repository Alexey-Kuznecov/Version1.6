
namespace UnityCommander.Common.Models.Directory
{
    using NSwag.Collections;
    using Prism.Mvvm;
    using System;
    using System.Collections.Generic;
    using UnityCommander.Common.Models.Icons;
    using UnityCommander.Common.Selection;

    /// <summary>
    /// The directory base.
    /// </summary>
    [Serializable]
    public abstract class BaseDirectory : BindableBase, ISelectableItem
    {
        private bool _isSelected;
        public string Name { get; set; }
        public string Path { get; set; }
        public Icon Icon { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastAccessTime { get; set; }
        public IDictionary<string, object> Additional { get; set; } = new ObservableDictionary<string, object>();
        public TargetPanel TargetPanel { get; set; }
        public List<ContextItem> ContextItems { get; set; }
        public string Key { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}