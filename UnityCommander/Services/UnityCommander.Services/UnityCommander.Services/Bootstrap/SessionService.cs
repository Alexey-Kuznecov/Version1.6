
using System.IO;
using System.Text.Json;
using UnityCommander.Common.State;
using UnityCommander.Services.Interfaces.Bootstrap;

namespace UnityCommander.Services.Bootstrap
{
    public class SessionService : ISessionService
    {
        private const string FileName = "session.json";

        public AppSessionState Load()
        {
            if (!File.Exists(FileName))
                return SessionDefaults.Create();

            var json = File.ReadAllText(FileName);

            if (string.IsNullOrWhiteSpace(json))
                return SessionDefaults.Create();

            var state = JsonSerializer.Deserialize<AppSessionState>(json);

            return state ?? SessionDefaults.Create();
        }

        public void Save(AppSessionState state)
        {
            if (state == null)
                return;

            var json = JsonSerializer.Serialize(state);
            File.WriteAllText(FileName, json);
        }
    }
}
