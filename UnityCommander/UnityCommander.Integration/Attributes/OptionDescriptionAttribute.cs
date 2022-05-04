
namespace UnityCommander.Integration.Attributes
{
    using System;

    /// <summary>
    /// Describes the plugin settings parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAttribute"/> class.
        /// </summary>
        /// <param name="description"> Description of the settings parameter.. </param>
        public OptionAttribute(string description)
        {
            this.Description = description;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }
    }
}
