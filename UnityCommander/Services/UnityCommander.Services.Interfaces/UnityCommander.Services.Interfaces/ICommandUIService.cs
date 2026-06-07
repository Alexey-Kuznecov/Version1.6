
using Prism.Commands;
using System;
using UnityCommander.Common.Commands;

namespace UnityCommander.Services.Interfaces
{
    public interface ICommandUIService
    {
        UICommand Create(string id);

        public UICommand Create<T>(
             string id,
             DelegateCommand<T> command,
             Func<bool> canExecute);
    }
}
