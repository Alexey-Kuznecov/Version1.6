
using System.Windows.Media;
using UnityCommander.Abstractions;
using UnityCommander.Diagnostics.Tracing;

namespace UnityCommander.Rendering.Icons
{
    public static class IconHub
    {
        private static IIconRenderService? _service;
        private static IIconColorResolver _iconColor;
        private static IDiagnosticTrace? _trace;

        public static void Initialize(
            IIconRenderService service,
            IIconColorResolver iconColor,
            IDiagnosticTrace trace)
        {
            _service = service;
            _iconColor = iconColor;
            _trace = trace;
        }

        public static IDiagnosticTraceScope? Begin(
            string source,
            string operation,
            IReadOnlyDictionary<string, object?>? data = null)
        {
            return _trace?.Begin(source, operation, data);
        }

        public static bool TryGet(
            string key,
            out IconRenderResult result)
        {
            result = default!;

            return _service != null &&
                   _service.TryGet(key, out result);
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
