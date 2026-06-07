using System;
using System.Collections.Generic;

namespace UnityCommander.Controls.Ribbon2.Models
{
    public class RibbonGroupModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public List<RibbonToolModel> Tools { get; set; } = new();
        public RibbonGroupBehavior Behavior { get; set; } = new();
    }
}
