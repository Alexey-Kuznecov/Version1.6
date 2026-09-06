
namespace UnityCommander.Abstractions.Diagnostic
{
    public class DebugNavigationState
    {
        public string? Current { get; set; }
        public int BackCount { get; set; }
        public int ForwardCount { get; set; }
        public bool CanGoBack { get; set; }
        public bool CanGoForward { get; set; }
    }
}
