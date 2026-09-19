
using NSwag.Collections;
using Prism.Mvvm;
using System.Collections.Generic;
using UnityCommander.Abstractions.Panels;

namespace UnityCommander.Common.Models.Directory
{
    public class CellModel : BindableBase, ICellModel
    {
        public IDictionary<string, int> DynamicColumnIndexes { get; }
       = new Dictionary<string, int>();

        public IDictionary<string, object> Additional { get; set; }
         = new ObservableDictionary<string, object>();
        
        public int _outputOrder;

        public int OutputOrder
        {
            get => _outputOrder;
            set => SetProperty(ref _outputOrder, value);
        }
    }
}
