
using UnityCommander.CLI.Core;

namespace UnityCommander.Commands.Rendering
{
    public interface IConsoleRenderer<T>
    {
        void Render(
            IConsoleOutput output,
            T model);
    }
}
