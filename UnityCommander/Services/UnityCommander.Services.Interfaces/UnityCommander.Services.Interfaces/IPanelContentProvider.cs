using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Common.Models.Directory;

namespace UnityCommander.Services.Interfaces
{
    public interface IPanelContentProvider
    {
        bool IsActive { get; }
        string PanelId { get; }
        string GetCurrentPath();
        IReadOnlyList<BaseDirectory> GetCurrentDirectoryFiles();
    }
}
