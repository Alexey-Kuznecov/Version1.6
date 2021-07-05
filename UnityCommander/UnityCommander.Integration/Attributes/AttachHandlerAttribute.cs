
namespace UnityCommander.Integration.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Enums;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The selector type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AttachHandlerAttribute : Attribute
    {
        /// <summary>
        /// The association actions.
        /// </summary>
        private readonly IDictionary<PluginScopes, Type> associationActions = new Dictionary<PluginScopes, Type>
            {
                { PluginScopes.Columns, typeof(AddColumnsDelegate) }
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachHandlerAttribute"/> class.
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
        public AttachHandlerAttribute(OptionRender render, Type classType, string handlerName, Type handler)
        {
            this.BaseHandler = Activator.CreateInstance(classType);
            var method = classType.GetMethod(handlerName)?.GetParameters();
            this.Handler = Delegate.CreateDelegate(handler, this.BaseHandler, handlerName);
            this.HandlerName = handlerName;
            this.OptionRender = render;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachHandlerAttribute"/> class.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="classType">
        /// The plugin
        /// </param>
        /// <param name="handlerName">
        /// The handler name
        /// </param>
        public AttachHandlerAttribute(PluginScopes target, Type classType, string handlerName)
        {
            this.BaseHandler = Activator.CreateInstance(classType);
            this.Handler = Delegate.CreateDelegate(this.associationActions.Single(k => k.Key == target).Value, this.BaseHandler, handlerName);
            this.HandlerName = handlerName;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public OptionRender OptionRender { get; set; }

        /// <summary>
        /// Gets or sets the option handler.
        /// </summary>
        public Delegate Handler { get; set; }

        /// <summary>
        /// Gets or sets the option handler.
        /// </summary>
        public string HandlerName { get; set; }

        /// <summary>
        /// Gets or sets the base handler.
        /// </summary>
        public object BaseHandler { get; set; }
    }
}
