
using System.Windows.Media;
using UnityCommander.Abstractions;

namespace UnityCommander.Rendering.Icons
{
    public static class IconHub
    {
        private static IIconRenderService? _service;

        private static IIconColorResolver _iconColor;

        public static void Initialize(IIconRenderService service, IIconColorResolver iconColor)
        {
            _service = service;
            _iconColor = iconColor;
        }

        public static bool TryGet(string key, out IconRenderResult result)
        {
            result = default!;

            return _service != null
                && _service.TryGet(key, out result);
        }

        public static Brush Resolve(
            IconKind kind,
            IconRole role,
            IconTone tone,
            VisualState state)
        {
            return _iconColor.Resolve(kind, role, tone, state);
        }
    }
}
