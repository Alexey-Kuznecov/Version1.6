using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UnityCommander.Commands.Performance
{
    public static class PhysicalDiskResolver
    {
        private static Dictionary<string, string> _map;

        public static void Initialize()
        {
            var cat = new PerformanceCounterCategory("PhysicalDisk");
            var instances = cat.GetInstanceNames();

            _map = BuildMap(instances);
        }

        private static Dictionary<string, string> BuildMap(string[] instances)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var instance in instances)
            {
                // "0 C:" → "C"
                if (instance.Contains(":"))
                {
                    var drive = instance.Split(' ').Last().Replace(":", "");
                    map[drive] = instance;
                }

                map[instance] = instance; // _Total, 0 C:
            }

            return map;
        }

        public static string Resolve(string input)
        {
            if (_map == null)
                Initialize();

            if (string.IsNullOrWhiteSpace(input))
                return "_Total";

            if (_map.TryGetValue(input, out var value))
                return value;

            throw new InvalidOperationException($"Disk '{input}' not found");
        }
    }
}
