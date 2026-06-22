
namespace UnityCommander.Rendering.Icons
{
    public static class IconHub
    {
        private static IIconRenderService? _service;

        public static void Initialize(IIconRenderService service)
        {
            _service = service;
        }

        public static bool TryGet(string key, out IconRenderResult result)
        {
            result = default!;

            return _service != null
                && _service.TryGet(key, out result);
        }
    }
}
