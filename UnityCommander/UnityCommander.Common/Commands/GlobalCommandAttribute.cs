
namespace UnityCommander.Common.Commands
{
    using System;
    using System.Windows.Input;

    [AttributeUsage(AttributeTargets.Method)]
    public class GlobalCommandAttribute : Attribute
    {
        public string Name { get; set; }
        
        public KeyGesture Hotkey { get; set; }

        public GlobalCommandAttribute(string name, string hotkey)
        {
            if (name == "FileCopy3")
            {
                throw new Exception("This name is used!");
            }

            this.Name = name;
            this.Hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString(hotkey);
        }
    }
}
