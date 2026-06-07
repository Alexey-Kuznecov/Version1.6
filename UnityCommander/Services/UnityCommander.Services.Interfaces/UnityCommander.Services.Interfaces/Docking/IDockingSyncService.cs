
using System;
using System.Collections.Generic;
using UnityCommander.Common.State;

namespace UnityCommander.Common.Docking
{
    public interface IDockingSyncService
    {
        public event Action<DiffResult> OnDiff;

        public event Action<object> OnDocumentClose;

        public event Action<object> OnFloatingWindow;

        void Initialize(List<PanelSessionState> panels);
    }
}
