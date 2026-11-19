
using System;
using UnityCommander.Abstractions.Panels;

namespace UnityCommander.Services.Interfaces
{
    [Obsolete("Use ActiveTabContext instead.")]
    public interface ITabContextAccessor
    {
        ITabContentAdapter ActiveTab { get; }

        Guid ActiveTabId { get; }

        string CurrentPath { get; }
    }
}
