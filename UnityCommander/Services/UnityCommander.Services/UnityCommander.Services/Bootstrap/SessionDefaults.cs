
using System;
using System.Collections.Generic;
using UnityCommander.Common.State;

namespace UnityCommander.Services.Bootstrap
{
    public static class SessionDefaults
    {
        public static AppSessionState Create()
        {
            return new AppSessionState
            {
                Panels = new List<PanelSessionState>
                {
                    new PanelSessionState
                    {
                        PanelId = Guid.NewGuid(),
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
    }
}
