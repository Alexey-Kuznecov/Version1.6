
using System;
using System.Collections.Generic;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Abstractions.Statusbar;
using UnityCommander.Common.Models;
using UnityCommander.Modules.StatusBar.Services;

namespace UnityCommander.Core.StatusBar
{
    public class DefaultStatusBarProvider : IStatusBarProvider
    {
        private readonly IFileStateService _fileState;

        public DefaultStatusBarProvider(IFileStateService fileState)
        {
            _fileState = fileState;
        }

        public IEnumerable<IStatusBarItem> CreateItems()
        {
            var dd = new List<IStatusBarItem>();

            return dd;
        }
    }
}
