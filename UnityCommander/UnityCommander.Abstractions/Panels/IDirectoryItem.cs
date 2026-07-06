
namespace UnityCommander.Abstractions.Panels
{
    using System;
    using System.Collections.Generic;
    using UnityCommander.Abstractions;
    using UnityCommander.Abstractions.Selection;

    /// <summary>
    /// The directory base.
    /// </summary>
    public interface IDirectoryItem : ISelectableItem
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string IconKey { get; set; }

        public IconKind Kind { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime LastAccessTime { get; set; }

        public IDictionary<string, object> Additional { get; set; }
        
        public TargetPanel TargetPanel { get; set; }
        
        public Dictionary<string, DateTime> LastUpdate { get; }

        public string Key { get; set; }

        public bool IsSelected { get; set; }
    }
}