
namespace UnityCommander.Common.Module
{
    using System.Collections.Generic;
    using UnityCommander.Common.Models.Directory;
    using UnityCommander.Common.Panels;

    public interface IDirectoryPanel : ITabPanelContent
    {
        IReadOnlyList<BaseDirectory> GetFiles();

        public IFileNodeContext FileContext { get; }
    }
}
