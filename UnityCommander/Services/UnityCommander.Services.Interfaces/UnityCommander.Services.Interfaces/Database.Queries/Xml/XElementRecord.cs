
namespace UnityCommander.Services.Interfaces.Database.Queries.Xml
{
    using System.Collections.Generic;

    /// <summary>
    /// The x element record.
    /// </summary>
    public record XElementRecord
    {
        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new ();
    }
}
