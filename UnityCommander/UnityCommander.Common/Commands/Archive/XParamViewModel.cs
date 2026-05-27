
using System;

namespace UnityCommander.Common.Commands
{
    [Obsolete]
    public class XParamViewModel
    {
        public object Instance { get; set; }
        
        public string PropertyName { get; set; }

        public XParamViewModel(object instance, string propertyName)
        {
            this.Instance = instance;
            this.PropertyName = propertyName;
        }
    }
}
