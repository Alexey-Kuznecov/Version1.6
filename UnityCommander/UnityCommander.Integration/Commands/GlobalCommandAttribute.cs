using System;
using System.Collections.Generic;
using System.Windows.Input;
using UnityCommander.Integration.Enums;

namespace UnityCommander.Integration.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GlobalCommandAttribute : Attribute
    {
        public string Name { get; set; }
        public CommandSource Source { get; set; }
        
        public KeyGesture Hotkey { get; set; }

        public GlobalCommandAttribute(string name, string hotkey, CommandSource source = CommandSource.Plugin)
        {
            if (name == "FileCopy3")
            {
                throw new Exception("This name is used!");
            }

            this.Name = name;
            this.Source = source;
            this.Hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString(hotkey);
        }
    }
}
