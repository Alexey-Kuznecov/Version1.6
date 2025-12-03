using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Services.Interfaces
{
    public interface IPanelRegistry
    {
        IPanelContentProvider ActivePanel { get; }
        IPanelContentProvider GetActivePanel();
        IReadOnlyList<IPanelContentProvider> GetAllPanels();
        IPanelContentProvider GetPanel(string panelId);

        void RegisterPanel(IPanelContentProvider provider);
        void UnregisterPanel(string panelId);

        void SetActivePanel(string panelId);
    }
}
