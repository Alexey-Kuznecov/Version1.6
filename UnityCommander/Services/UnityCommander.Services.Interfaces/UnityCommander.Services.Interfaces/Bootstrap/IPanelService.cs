
using System.Collections.Generic;
using UnityCommander.Common.State;

namespace UnityCommander.Services.Interfaces.Bootstrap
{
    public interface IPanelService
    {
        void Initialize();
        void Restore(List<PanelState> panels);
    }
}
