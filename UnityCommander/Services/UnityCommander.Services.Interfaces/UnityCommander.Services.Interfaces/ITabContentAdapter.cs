
using System;
using System.Collections.Generic;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Common.Module;

namespace UnityCommander.Services.Interfaces
{
    public interface ITabContentAdapter : IAttachAware, IDisposable
    {
        bool IsActive { get; }
        Guid TabId { get; }
        string GetCurrentPath();
        IReadOnlyList<BaseDirectory> GetCurrentDirectoryFiles();
    }
}
