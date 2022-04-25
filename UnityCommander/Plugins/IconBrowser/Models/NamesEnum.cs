
namespace AIconBrowser.Models
{
    /// <summary>
    /// Names of icon collection.
    /// </summary>
    enum NamesEnum : byte
    {
        Unsigned = 0,
        Game = 1
    }
    static class Names
    {
        /// <summary>
        /// Returns the Russian version of the name of the collection menu.
        /// </summary>
        public static string GetName(this NamesEnum code)
        {
            switch (code)
            {
                case NamesEnum.Unsigned:
                    return "Неподшитые";
                case NamesEnum.Game:
                    return "Игры";
            }
            return null;
        }
    }
}
