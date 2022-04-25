using System;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Common
{
    public class UCCommandAttribute : Attribute
    {
        public string Name { get; set; }

        public UCCommandAttribute()
        {
        }

        public UCCommandAttribute(string name)
        {
            this.Name = name;
        }
    }
}
