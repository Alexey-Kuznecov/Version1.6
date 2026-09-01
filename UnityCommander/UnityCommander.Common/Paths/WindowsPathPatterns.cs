
namespace UnityCommander.Common.Paths
{
    public static class WindowsPathPatterns
    {
        public const string AbsolutePath =
            @"(?<![A-Za-z0-9_.-])" +
            @"[A-Za-z]:\\" +
            @"(?:[^<>:""/\\|?*\r\n]+\\)*" +
            @"[^<>:""/\\|?*\r\n]+" +
            @"(?=$|[\s""')\]},;])";

        public const string AbsolutePath2 =
    @"(?<![A-Za-z0-9_.-])" +
    @"[A-Za-z]:\\" +
    @"(?:[^<>:""/\\|?*\r\n]+\\)*?" +
    @"[^<>:""/\\|?*\r\n]+?" +
    @"(?=$|[\s""')\]},;])";
    }
}
