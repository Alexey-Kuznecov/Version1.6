using System;

namespace UnityCommander.Integration.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GlobalCommandAttribute : Attribute
    {
        public string Name { get; set; }
        public CommandSource Source { get; set; }

        public GlobalCommandAttribute(string name, CommandSource source = CommandSource.Plugin)
        {
            if (name == "FileCopy3")
            {
                throw new Exception("This name is used!");
            }

            this.Name = name;
            this.Source = source;
        }
    }
}
