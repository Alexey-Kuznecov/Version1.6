using System;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Integration.Contracts
{
    public interface IPluginDescriptor
    { 
        /// <summary>
        /// Gets or sets plugin name.
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// Gets or sets plugin description.
        /// </summary>
        public string Description { get; set; }
    }
}
