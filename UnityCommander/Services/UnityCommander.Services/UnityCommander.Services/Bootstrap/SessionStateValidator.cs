
using System.IO;
using UnityCommander.Common.State;

namespace UnityCommander.Services.Bootstrap
{
    public sealed class SessionStateValidator
    {
        public AppSessionState Validate(AppSessionState state)
        {
            foreach (var panel in state.Panels)
            {
                panel.Tabs.RemoveAll(tab => !Directory.Exists(tab.Path));
            }

            return state;
        }
    }
}
