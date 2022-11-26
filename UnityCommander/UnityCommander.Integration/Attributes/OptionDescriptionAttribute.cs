
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
        public OptionAttribute(string title, string description, string category)
        {
            this.Category = category;
            this.Description = description;
            this.Title = title;
        }

        /// <summary>
        /// Категория и тег которая используется для классификации параметров.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Название параметра или краткое описания.
        /// </summary>
        public string Title { get; set; }
    }
}
