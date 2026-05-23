
using System;
using System.Collections.Generic;

namespace UnityCommander.Common.Docking
{
    //public class DiffResult
    //{
    //    public List<(string panelId, string tabId)> AddedTabs = new();
    //    public List<(string panelId, string tabId)> RemovedTabs = new();
    //    public List<(string from, string to, string tabId)> MovedTabs = new();
    //}

    public class DiffResult
    {
        public List<Guid> AddedTabs { get; set; } = new();
        public List<Guid> RemovedTabs { get; set; } = new();
        public List<MoveTab> MovedTabs { get; set; } = new();
    }
}
