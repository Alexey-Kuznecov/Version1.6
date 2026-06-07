
using System;

namespace UnityCommander.Services.Interfaces
{
    public interface ISelectionService
    {
        void Register(Guid tabId, ISelectionManager manager);

        void Unregister(Guid tabId);

        ISelectionManager Get(Guid tabId);

        ISelectionManager GetActive();
    }
}
