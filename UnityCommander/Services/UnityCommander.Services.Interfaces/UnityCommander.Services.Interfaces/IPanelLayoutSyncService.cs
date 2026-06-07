using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Services.Interfaces
{
    public interface IPanelLayoutSyncService
    {
        event Action<double> SplitterChanged;

        double SplitterPosition { get; set; }

        void Update(double value);
    }
}
