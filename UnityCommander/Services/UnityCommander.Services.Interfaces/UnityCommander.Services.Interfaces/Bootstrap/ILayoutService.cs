
using UnityCommander.Common.State;

namespace UnityCommander.Services.Interfaces.Bootstrap
{
    public interface ILayoutService
    {
        void Load(AppSessionState session);
        void Save();
    }
}
