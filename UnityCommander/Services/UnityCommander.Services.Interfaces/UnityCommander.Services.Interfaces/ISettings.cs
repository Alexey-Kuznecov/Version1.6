
namespace UnityCommander.Services.Interfaces
{
    using System;

    /// <summary>
    /// The Settings interface.
    /// </summary>
    public interface ISettings
    {
        /// <summary>
        /// Gets a value indicating whether sidebar display content.
        /// </summary>
        bool SidebarDisplayContent { get; }

        /// <summary>
        /// Gets the value indicating whether the session will be saved when the program is closed.
        /// </summary>
        bool IsSessionSaved { get; }

        /// <summary>
        /// Gets the session files.
        /// </summary>
        string SessionFiles { get; }
    }
}
