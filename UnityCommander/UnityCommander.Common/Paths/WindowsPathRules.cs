
namespace UnityCommander.Common.Paths
{
    public static class WindowsPathRules
    {
        public const string InvalidFileNameCharacters =
            "<>:\"/\\|?*";

        public static bool IsValidFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (name.IndexOfAny(
                    InvalidFileNameCharacters.ToCharArray()) >= 0)
                return false;

            return true;
        }

        public static bool IsValidPathCharacter(char c)
            => c != '<'
            && c != '>'
            && c != ':'
            && c != '"'
            && c != '/'
            && c != '\\'
            && c != '|'
            && c != '?'
            && c != '*';
    }
}
