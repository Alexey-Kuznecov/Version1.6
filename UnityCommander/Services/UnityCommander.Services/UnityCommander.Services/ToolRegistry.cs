
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
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            return _tools.GetValueOrDefault(id);
        }

        public IToolDescriptor? FindByContentId(string contentId)
        {
            if (string.IsNullOrWhiteSpace(contentId))
                return null;

            // Сначала проверяем точное совпадение.
            var descriptor = Get(contentId);

            if (descriptor != null)
                return descriptor;

            // Формат экземпляра: ToolId:InstanceId.
            var separatorIndex = contentId.IndexOf(':');

            if (separatorIndex <= 0)
                return null;

            var toolId = contentId[..separatorIndex];

            return Get(toolId);
        }

        public IReadOnlyCollection<IToolDescriptor> GetAll() 
            => _tools.Values.ToList();
    }
}
