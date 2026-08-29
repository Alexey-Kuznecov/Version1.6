
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public sealed class ToolRegistry : IToolRegistry
    {
        private readonly Dictionary<string, IToolDescriptor> _tools;

        public ToolRegistry(IEnumerable<IToolDescriptor> tools)
        {
            _tools = tools.ToDictionary(
                x => x.Id,
                StringComparer.OrdinalIgnoreCase);
        }

        public IToolDescriptor? Get(string id)
            => _tools.GetValueOrDefault(id);

        public IToolDescriptor? FindByContentId(string contentId)
        {
            foreach (var descriptor in _tools.Values)
            {
                if (contentId.Contains(
                        descriptor.Id,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return descriptor;
                }
            }

            return null;
        }
    }
}
