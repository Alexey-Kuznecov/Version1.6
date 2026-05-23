using System;
using System.Collections.Generic;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Common.Module;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class TabContentAdapter : ITabContentAdapter
    {
        private readonly ITabPanelContent _vm;

        public TabContentAdapter(ITabPanelContent vm)
        {
            _vm = vm ?? throw new ArgumentNullException(nameof(vm));
        }

        public Guid TabId => _vm.GetPanelToken(); // ← просто переименуй смысл

        public bool IsActive => _vm.IsActive;

        public string GetCurrentPath() => _vm.GetCurrentPath();

        public IReadOnlyList<BaseDirectory> GetCurrentDirectoryFiles()
        {
            if (_vm is IDirectoryPanel dp)
                return dp.GetFiles();

            throw new NotSupportedException();
        }
    }
}
