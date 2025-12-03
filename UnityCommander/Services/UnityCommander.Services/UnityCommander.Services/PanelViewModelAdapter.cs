using System;
using System.Collections.Generic;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Common.Module;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class PanelViewModelAdapter : IPanelContentProvider
    {
        private readonly ITabPanelContent _vm; // или IDirectoryPanel, в зависимости что у тебя
        public PanelViewModelAdapter(ITabPanelContent vm)
        {
            _vm = vm ?? throw new ArgumentNullException(nameof(vm));
        }

        public string PanelId => _vm.GetPanelToken().ToString();

        public bool IsActive => throw new NotImplementedException();

        public string GetCurrentPath() => _vm.GetCurrentPath();

        public IReadOnlyList<BaseDirectory> GetCurrentDirectoryFiles()
        {
            // если у VM нет явного метода возвращения файлов, добавь свойство Files в VM или используй FileSystem внутри VM:
            // пример:
            if (_vm is IDirectoryPanel dp)
            {
                // тут предполагаем, что у IDirectoryPanel есть доступ к файловой системе или коллекции
                // если нет — тебе нужно добавить в VM метод/свойство, которое отдаёт файлы
                return dp.GetFiles(); // <- добавить в IDirectoryPanel, если надо
            }

            // fallback — попытка через reflection/explicit API
            throw new NotSupportedException("VM does not expose files; adapt VM to provide them.");
        }
    }
}
