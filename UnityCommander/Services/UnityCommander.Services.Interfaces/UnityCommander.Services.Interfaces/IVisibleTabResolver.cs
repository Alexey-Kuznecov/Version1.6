
using System;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Services.Interfaces
{
    public interface IVisibleTabResolver
    {
        IReadOnlyCollection<Guid> GetVisibleTabs();
    }
}
