
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                return CreateDefault();

            var json = File.ReadAllText(FileName);

            if (string.IsNullOrWhiteSpace(json))
                return CreateDefault();

            var state = JsonSerializer.Deserialize<AppSessionState>(json);

            return IsValid(state) ? state : CreateDefault();
        }

        public void Save(AppSessionState state)
        {
            var json = JsonSerializer.Serialize(state);
            File.WriteAllText(FileName, json);
        }

        private AppSessionState CreateDefault()
        {
            return new AppSessionState
            {
                Panels = new List<PanelSessionState>()
                {
                    new PanelSessionState() 
                    {
                         Tabs = new List<TabSessionState>
                        {
                            new TabSessionState
                            {
                                Title = "C:\\",
                                Path = "C:\\",
                                TabId = Guid.NewGuid()
                            }
                        }
                    }
                }
            };
        }

        private bool IsValid(AppSessionState state)
        {
            if (state == null)
                return false;

            if (state.Panels == null || state.Panels.Count == 0)
                return false;

            return state.Panels.Any(p => p.Tabs != null && p.Tabs.Count > 0);
        }
    }
}
