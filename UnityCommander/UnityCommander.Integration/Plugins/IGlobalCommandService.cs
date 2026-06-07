
namespace UnityCommander.Services.Interfaces
{
    using Common;
    using System;
    using UnityCommander.Common.Commands;

    [Obsolete]
    public interface IGlobalCommandService
    {
        IGlobalCommandManager GetCommandManager();
    }
}
