
using System;
using System.Collections.Generic;
using UnityCommander.Common.State;

namespace UnityCommander.Common.Docking
{
    public interface IDockingSyncService
    {
        public event Action<DiffResult> OnDiff;
        void Initialize(List<PanelState> panels);
    }
}
