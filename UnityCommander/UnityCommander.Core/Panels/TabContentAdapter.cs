
using System;
using System.Collections.Generic;
using UnityCommander.Abstractions.Module;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Common.Models.Directory;

namespace UnityCommander.Services
{
    public class TabContentAdapter : ITabContentAdapter
    {
        private readonly ITabPanelContent _vm;

        public event Action<string> PathChanged;

        public TabContentAdapter(ITabPanelContent vm)
        {
            _vm = vm ?? throw new ArgumentNullException(nameof(vm));
            _vm.PathChanged += OnPathChanged;
        }

        public Guid TabId => _vm.GetPanelToken(); // ← просто переименуй смысл

        public bool IsActive => _vm.IsActive;

        public string GetCurrentPath() => _vm.GetCurrentPath();

        private void OnPathChanged(string path)
        {
            PathChanged?.Invoke(path);
        }

        public IReadOnlyList<IDirectoryItem> GetCurrentDirectoryFiles()
        {
            if (_vm is IDirectoryPanel dp)
                return dp.GetFiles();

            throw new NotSupportedException();
        }

        public void OnAttached(object view)
        {
            if (_vm is IViewAttachAware aware)
                aware.OnViewAttached(view);
        }

        public void OnDetached()
        {
            if (_vm is IViewAttachAware aware)
                aware.OnViewDetached();
        }

        public void Dispose()
        {
            _vm.PathChanged -= OnPathChanged;
            _vm.Dispose();
        }

        public IDirectoryPanel GetContent()
            => (IDirectoryPanel)_vm;
    }
}
