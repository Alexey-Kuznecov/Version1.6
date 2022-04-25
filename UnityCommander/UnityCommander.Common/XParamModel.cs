using System;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Common
{
    public class XParamModel
    {
        public object Instance { get; set; }
        
        public string PropertyName { get; set; }

        public XParamModel(object instance, string propertyName)
        {
            this.Instance = instance;
            this.PropertyName = propertyName;
        }
    }
}
