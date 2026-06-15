
namespace UnityCommander.Common.Models.Directory
{
    using Prism.Mvvm;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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

        public IDictionary<string, object> Additional { get; set; }
            = new NSwag.Collections.ObservableDictionary<string, object>();

        //public IDictionary<string, object> Additional => _additional;

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
      
        //private object _value;

        //public object Value
        //{
        //    get => _value;
        //    set => SetProperty(ref _value, value);
        //}

        //public Dictionary<string, ColumnValue> ColumnValues { get; }
        //        = new();

        //public ObservableCollection<CellModel> Cells { get; }

        //public ObservableCollection<ColumnValue> ColumnValues { get; }
        //= new();

        //public List<ColumnValue> ColumnValues { gets; }

        //public object GetColumnValue(string id)
        //{
        //    _additional.TryGetValue(id, out var value);
        //    return value;
        //}

        //public void SetColumnValue(string id, object value)
        //{
        //    _additional[id] = value;
        //    RaisePropertyChanged($"Column:{id}");
        //}
    }
}