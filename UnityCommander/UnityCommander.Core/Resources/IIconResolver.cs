
using UnityCommander.Common.Models.Icons;

namespace UnityCommander.Core.Resources
{
    public interface IIconResolver
    {
        public bool TryResolve(string key,
            out IIcon icon);
    }
}