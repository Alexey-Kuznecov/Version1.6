
using System;
using System.Buffers;

namespace UnityCommander.Common.Paths
{
    public static class WindowsFileNameValidator
    {
        private static readonly SearchValues<char> InvalidCharacters =
            SearchValues.Create("<>:\"/\\|?*");

        public static bool IsValid(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (name.AsSpan().IndexOfAny(InvalidCharacters) >= 0)
                return false;

            return true;
        }
    }
}
