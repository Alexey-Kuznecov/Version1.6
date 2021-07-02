
using System;
using UnityCommander.Integration.Contracts;
using UnityCommander.Integration.Enums;

namespace UnityCommander.Integration.Attributes
{
    /// <summary>
    /// The selector type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ValueHandlerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueHandlerAttribute"/> class.
        /// </summary>
        /// <param name="render">
        /// The selector.
        /// </param>
        /// <param name="classType">
        /// The plugin
        /// </param>
        /// <param name="handlerName">
        /// The handler name
        /// </param>
        /// <param name="handler">
        /// The handler.
        /// </param>
        public ValueHandlerAttribute(OptionRender render, TargetPanel targetPanel, Type classType, string handlerName, Type handler)
        {
            this.BaseHandler = Activator.CreateInstance(classType);
            this.OptionHandler = Delegate.CreateDelegate(handler, this.BaseHandler, handlerName);
            this.OptionHandlerName = handlerName;
            this.OptionRender = render;
        }

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
