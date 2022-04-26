using System;

namespace UnityCommander.Integration.Commands
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
