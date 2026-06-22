using System.Windows.Shapes;

namespace UnityCommander.Rendering.Icons
{
    public interface IIconRenderService
    {
        bool TryGet(string key, out IconRenderResult result);

        public Path GetPath(string key);
    }
}