
namespace UnityCommander.Common.Module
{
    using System;

    /// <summary>
    /// The DirectoryPanel interface.
    /// </summary>
    public interface IDirectoryPanel : ITabPanelContent
    {
        public void DirectoryUpdate(IDirectoryPanel directoryPanel);
    }
}
