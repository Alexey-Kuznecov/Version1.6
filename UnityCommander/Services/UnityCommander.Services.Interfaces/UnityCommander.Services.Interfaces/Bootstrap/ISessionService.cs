

using UnityCommander.Common.State;

namespace UnityCommander.Services.Interfaces.Bootstrap
{
    public interface ISessionService
    {
        AppSessionState Load();
        void Save(AppSessionState state);
    }
}
