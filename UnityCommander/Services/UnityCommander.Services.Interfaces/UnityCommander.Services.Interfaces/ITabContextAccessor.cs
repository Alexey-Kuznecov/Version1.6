
using System;

namespace UnityCommander.Services.Interfaces
{
    public interface ITabContextAccessor
    {
        ITabContentAdapter ActiveTab { get; }

        Guid ActiveTabId { get; }

        string CurrentPath { get; }
    }
}
