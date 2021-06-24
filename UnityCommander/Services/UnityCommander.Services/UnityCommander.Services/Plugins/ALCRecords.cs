using System;
using System.Collections.Generic;
using System.Reflection;

using System.Text;

namespace UnityCommander.Services.Plugins
{
#if NETCOREAPP3_1
    using System.Runtime.Loader;
#endif

    public record ALCRecords
    {
#if NETCOREAPP3_1
        /// <summary>
        /// Gets or sets plugin assembly name.
        /// </summary>
        public AssemblyLoadContext Alc { get; set; }
#endif
        
        /// <summary>
        /// Gets or sets plugin assembly name.
        /// </summary>
        public AssemblyName AssemblyName { get; set; }

        /// <summary>
        /// Gets or sets plugin token.
        /// </summary>
        public Guid Token { get; set; } = Guid.NewGuid();
    }
}
