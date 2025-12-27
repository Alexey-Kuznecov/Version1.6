namespace UnityCommander.Logging.Contracts
{
    public interface ILogger
    {
        void Trace(string message);
        void Debug(string message);
        void Info(string message);
        void ObjectInfo(string message, object obj);
        void Warning(string message);
        void Error(string message, Exception? ex = null);
        void Fatal(string message, Exception? ex = null);
    }
}