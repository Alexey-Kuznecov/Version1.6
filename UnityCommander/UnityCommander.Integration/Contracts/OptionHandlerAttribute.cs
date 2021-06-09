
namespace UnityCommander.Integration.Contracts
{
    using System;

    using UnityCommander.Integration.Contracts.Columns;

    /// <summary>
    /// The selector type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionHandlerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionHandlerAttribute"/> class.
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
        public OptionHandlerAttribute(OptionRender render, Type classType, string handlerName, Type handler)
        {
            this.BaseHandler = Activator.CreateInstance(classType);
            this.OptionHandler = Delegate.CreateDelegate(handler, this.BaseHandler, handlerName);
            this.OptionHandlerName = handlerName;
            this.OptionRender = render;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionHandlerAttribute"/> class.
        /// </summary>
        /// <param name="classType">
        /// The plugin
        /// </param>
        /// <param name="handlerName">
        /// The handler name
        /// </param>
        public OptionHandlerAttribute(Type classType, string handlerName)
        {
            this.BaseHandler = Activator.CreateInstance(classType);
            this.OptionHandler = Delegate.CreateDelegate(typeof(GetColumnsDelegate), this.BaseHandler, handlerName);
            this.OptionHandlerName = handlerName;
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
