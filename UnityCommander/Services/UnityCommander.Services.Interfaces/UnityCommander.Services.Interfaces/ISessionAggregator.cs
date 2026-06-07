
using System;
using UnityCommander.Common.State;

namespace UnityCommander.Services.Interfaces
{
    public interface ISessionAggregator
    {
        void RegisterCapture(Action<AppSessionState> capture);

        void RegisterRestore(Action<AppSessionState> restore);

        void Capture(AppSessionState state);

        void Restore(AppSessionState state);
    }
}
