
using UnityCommander.Common.State;

namespace UnityCommander.Services.Interfaces.Bootstrap
{
    public interface ISessionBuilder
    {
        AppSessionState Build();
    }
}
