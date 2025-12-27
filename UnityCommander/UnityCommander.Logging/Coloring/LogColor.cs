namespace UnityCommander.Logging.Coloring
{
    public readonly struct LogColor
    {
        public string Key { get; }

        public LogColor(string key)
        {
            Key = key;
        }

        public override string ToString() => Key;

        public static readonly LogColor Default = new("default");
        public static readonly LogColor Plugin = new("plugin");
        public static readonly LogColor System = new("system");
        public static readonly LogColor Autocomplete = new("autocomplete");
        public static readonly LogColor UserAction = new("user.action");
        public static readonly LogColor Error = new("error");
    }
}