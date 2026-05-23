
using System;

namespace UnityCommander.Common.Docking
{
    public interface IDockingSyncService
    {
        public event Action<DiffResult> OnDiff;
        void Initialize();
    }
}
