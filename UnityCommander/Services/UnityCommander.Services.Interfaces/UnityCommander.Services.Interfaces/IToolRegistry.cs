
using System.Collections.Generic;

namespace UnityCommander.Services.Interfaces
{
    public interface IToolRegistry
    {
        IToolDescriptor FindByContentId(string contentId);

        IToolDescriptor? Get(string id);

        IReadOnlyCollection<IToolDescriptor> GetAll();
    }
}