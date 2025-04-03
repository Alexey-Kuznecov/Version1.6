using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Common;

namespace UnityCommander.Integration
{
    /// <summary>
    /// Определяет минимальный набор методов для взаимодействия между плагинами (модуль Integration)  
    /// и бизнес-логикой приложения (модуль Core).  
    /// Класс служит мостом, позволяя Integration вызывать нужные операции из UnityCommander.Core  
    /// без жёсткой привязки к его реализации.  
    /// </summary>
    public class PluginBridge
    {
        private readonly IFileOperations _fileOperations;

        public PluginBridge(IFileOperations fileOperations)
        {
            _fileOperations = fileOperations;
        }

        public void ExecuteCreate()
        {
            _fileOperations.Create("source.txt");
        }
    }
}
