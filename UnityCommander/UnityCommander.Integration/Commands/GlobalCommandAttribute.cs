using System;

namespace UnityCommander.Integration.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GlobalCommandAttribute : Attribute
    {
        public string Name { get; set; }

        public GlobalCommandAttribute()
        {
        }

        public GlobalCommandAttribute(string name)
        {
            if (name == "FileCopy3")
            {
                throw new Exception("This name is used!");
            }

            this.Name = name;
        }
    }
}
