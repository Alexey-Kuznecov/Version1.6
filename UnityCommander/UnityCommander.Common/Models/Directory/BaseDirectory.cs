
namespace UnityCommander.Common.Models.Directory
{
    using Prism.Mvvm;
    using System;
    using System.Collections.Generic;
    using UnityCommander.Common.Selection;
    using UnityCommander.Rendering.Icons;

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
        public string IconKey { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastAccessTime { get; set; }

        public IDictionary<string, object> Additional { get; set; }
            = new NSwag.Collections.ObservableDictionary<string, object>();

        public TargetPanel TargetPanel { get; set; }
        public List<ContextItem> ContextItems { get; set; }
        
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