
using UnityCommander.Abstractions.Icons;

namespace UnityCommander.Rendering.Icons
{
    public interface IIconResolver
    {
        bool TryResolve(string key,
            out RuntimeIcon icon);

        RuntimeIcon Resolve(string key);
    }
}
