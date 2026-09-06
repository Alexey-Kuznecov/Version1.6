namespace UnityCommander.Logging.Core
{
    public readonly struct LogScope
    {
        public string OperationId { get; }

        public string Value { get; }

        private LogScope(string value)
        {
            Value = value;
        }

        public static readonly LogScope Startup = new("Startup");
        public static readonly LogScope Runtime = new("Runtime");
        public static readonly LogScope UI = new("UI");
        public static readonly LogScope UserAction = new("UserAction");

        public static LogScope Plugin()
            => new($"Plugin");

        public override string ToString() => Value;
    }
}
