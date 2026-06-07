
namespace UnityCommander.Integration.Commands
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// The global command attribute.
    /// </summary>
    public class GlobalCommandAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalCommandAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="hotkey">
        /// The hotkey.
        /// </param>
        public GlobalCommandAttribute(string name, string hotkey)
        {
            if (name == "FileCopy3")
            {
                throw new Exception("This name is used!");
            }

            this.Name = name;
            this.Hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString(hotkey);
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the hotkey.
        /// </summary>
        public KeyGesture Hotkey { get; set; }
    }
}
