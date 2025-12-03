using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Common.Selection;

namespace UnityCommander.Services.Interfaces
{
    public interface ISelectionService
    {
        void Register(string panelId, ISelectionManager manager);
        void Unregister(string panelId);
        ISelectionManager Get(string panelId);
        ISelectionManager GetActive();       // последний активный
        void SetActive(string panelId);
    }
}
