
namespace UnityCommander.Integration.Factories
{
    using System.Collections.Generic;

    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The associated types.
    /// </summary>
    public class AssociatedTypes
    {
        /// <summary>
        /// Gets or sets the registry dictionary.
        /// </summary>
        public Dictionary<IPluginService, object> Types { get; set; } = new ();
    }
}
