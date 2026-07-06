
namespace UnityCommander.Abstractions.Panels
{
    using System.Collections.Generic;

    public interface IDirectoryPanel : ITabPanelContent
    {
        IReadOnlyList<IFileItem> GetFiles();

        public IFileNodeContext FileContext { get; }
    }
}
