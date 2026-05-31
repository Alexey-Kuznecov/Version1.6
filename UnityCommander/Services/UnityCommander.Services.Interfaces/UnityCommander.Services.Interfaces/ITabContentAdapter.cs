
using System;
using System.Collections.Generic;
using UnityCommander.Common.Models.Directory;

namespace UnityCommander.Services.Interfaces
{
    public interface ITabContentAdapter : IDisposable
    {
        bool IsActive { get; }
        Guid TabId { get; }
        string GetCurrentPath();
        IReadOnlyList<BaseDirectory> GetCurrentDirectoryFiles();
    }
}
