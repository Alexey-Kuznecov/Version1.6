
namespace UnityCommander.Core.Helper
{
    public static class CommandCategoryHelper
    {
        public static string GetCategory(string commandId)
        {
            var parts = commandId.Split('.');

            return parts.Length > 1
                ? parts[0]
                : "General";
        }
    }
}
