
using System;
using System.Collections.Generic;

namespace UnityCommander.Services.Interfaces
{
    public interface IVisibleTabResolver
    {
        IReadOnlyCollection<Guid> GetVisibleTabs();
    }
}
