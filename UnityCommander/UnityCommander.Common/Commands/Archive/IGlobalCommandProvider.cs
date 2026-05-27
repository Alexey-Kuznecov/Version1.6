
using System;

namespace UnityCommander.Common.Commands
{
    [Obsolete]
    public interface IGlobalCommandProvider
    {
        /// <summary>
        /// The get command manager.
        /// </summary>
        /// <returns>
        /// The <see cref="IGlobalCommandManager"/>.
        /// </returns>
        IGlobalCommandManager GetCommandManager();
    }
}
