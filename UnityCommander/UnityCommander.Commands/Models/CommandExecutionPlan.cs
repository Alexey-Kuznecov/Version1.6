using System;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Commands.Models
{
    public class CommandExecutionPlan
    {
        public bool Watch { get; set; }

        public int IntervalMs { get; set; } = 500;

        public string Format { get; set; } = "table";
    }
}
