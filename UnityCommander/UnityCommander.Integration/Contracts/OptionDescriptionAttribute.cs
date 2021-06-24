
namespace UnityCommander.Integration.Contracts
{
    using System;

    /// <summary>
    /// Describes the plugin settings parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionDescriptionAttribute"/> class.
        /// </summary>
        /// <param name="description"> Description of the settings parameter.. </param>
        public OptionDescriptionAttribute(string description)
        {
            this.Description = description;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }
    }
}
