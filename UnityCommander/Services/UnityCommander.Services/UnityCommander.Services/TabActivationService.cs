

using System;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public sealed class TabActivationService
        : ITabActivationService
    {
        private readonly IDockingService _dockingService;

        public TabActivationService(
            IDockingService dockingService)
        {
            _dockingService = dockingService;
        }

        public bool Activate(Guid tabId)
        {
            var document = _dockingService.FindDocument(tabId);

            if (document == null)
                return false;

            _dockingService.Activate(document);

            return true;
        }
    }
}
