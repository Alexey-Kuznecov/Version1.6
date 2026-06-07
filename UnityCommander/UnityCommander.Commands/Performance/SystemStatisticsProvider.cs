using System;
using UnityCommander.Commands.Models;

namespace UnityCommander.Commands.Performance
{
    internal class SystemStatisticsProvider : ISystemStatisticsProvider
    {
        private SystemPerformanceMonitor monitor;

        public SystemStatisticsProvider(SystemPerformanceMonitor monitor)
        {
            this.monitor = monitor;
        }

        public SystemStatistics GetStatistics()
        {
            return GetStatistics();
        }
    }
}