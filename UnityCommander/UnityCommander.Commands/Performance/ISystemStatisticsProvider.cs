
using UnityCommander.Commands.Models;

namespace UnityCommander.Commands.Performance
{
    public interface ISystemStatisticsProvider
    {
        SystemStatistics GetStatistics();
    }
}
