using System;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Common
{
    public class GlobalCommandAttribute : Attribute
    {
        public string Name { get; set; }

        public GlobalCommandAttribute()
        {
        }

        public GlobalCommandAttribute(string name)
        {
            this.Name = name;
        }
    }
}
