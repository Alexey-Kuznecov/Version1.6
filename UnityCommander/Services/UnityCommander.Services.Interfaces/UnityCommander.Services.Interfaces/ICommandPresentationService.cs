
using UnityCommander.Common.Commands;

namespace UnityCommander.Services.Interfaces
{
    public interface ICommandPresentationService
    {
        UICommand Create(string commandId);
    }
}
