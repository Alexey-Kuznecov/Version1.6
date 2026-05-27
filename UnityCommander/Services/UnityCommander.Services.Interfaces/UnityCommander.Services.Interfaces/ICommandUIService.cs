
using Prism.Commands;
using System;
using UnityCommander.Common.Commands;

namespace UnityCommander.Services.Interfaces
{
    public interface ICommandUIService
    {
        UICommand Create(string id);

        public UICommand Create(
             string id,
             DelegateCommand command,
             Func<bool> canExecute);
    }
}
