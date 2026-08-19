

using System;

namespace UnityCommander.Services.Interfaces
{
    public interface ITabActivationService
    {
        bool Activate(Guid tabId);
    }
}
