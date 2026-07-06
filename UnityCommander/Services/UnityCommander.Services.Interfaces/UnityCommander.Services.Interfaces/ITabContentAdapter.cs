
using System;
using System.Collections.Generic;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Common.Module;

namespace UnityCommander.Services.Interfaces
{
    public interface ITabContentAdapter : IAttachAware, IDisposable
    {
        event Action<string> PathChanged;
        bool IsActive { get; }
        Guid TabId { get; }
        string GetCurrentPath();
        IReadOnlyList<BaseDirectory> GetCurrentDirectoryFiles();
        IDirectoryPanel GetContent();
    }
}
