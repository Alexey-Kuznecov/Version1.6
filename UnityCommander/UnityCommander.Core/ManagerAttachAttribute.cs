
namespace UnityCommander.Core
{
    using System;

    /// <summary>
    /// The command attribute.
    /// </summary>
    public class ManagerAttachAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerAttachAttribute"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public ManagerAttachAttribute(Type manager)
        {
            this.Function = manager;
        }

        /// <summary>
        /// Gets or sets the function.
        /// </summary>
        public Type Function { get; set; }
    }
}
