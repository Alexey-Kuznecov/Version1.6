
namespace UnityCommander.Integration.Attributes
{
    using System;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The selector type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ValueHandlerAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the base handler.
        /// </summary>
        public object BaseHandler { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public OptionRender OptionRender { get; set; }

        /// <summary>
        /// Gets or sets the option handler.
        /// </summary>
        public Delegate OptionHandler { get; set; }

        /// <summary>
        /// Gets or sets the option handler.
        /// </summary>
        public string OptionHandlerName { get; set; }
    }
}
