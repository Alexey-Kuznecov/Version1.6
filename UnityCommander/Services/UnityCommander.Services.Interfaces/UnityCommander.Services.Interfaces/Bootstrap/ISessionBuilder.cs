
using UnityCommander.Common.State;

namespace UnityCommander.Services.Interfaces.Bootstrap
{
    public interface ISessionBuilder
    {
        void Build(AppSessionState state);
    }
}
