using System;

namespace UnityCommander.Services.Interfaces
{
    public interface IServiceScopeResolver
    {
        IServiceProvider Resolve(string ownerId);
    }
}