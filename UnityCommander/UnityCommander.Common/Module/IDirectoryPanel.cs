
namespace UnityCommander.Common.Module
{
    using System;
    using System.Collections.Generic;
    using UnityCommander.Common.Models.Directory;

    /// <summary>
    /// The DirectoryPanel interface.
    /// </summary>
    public interface IDirectoryPanel : ITabPanelContent
    {
        IReadOnlyList<BaseDirectory> GetFiles();
    }
}
