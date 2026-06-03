using System.Collections;

namespace UnityCommander.Logging.Contracts
{
    public interface ILogger
    {
        void Trace(string message);
        
        void Debug(string message);
       
        void Debug(string message, Func<bool>? condition = null);
        
        void Info(string message);
       
        void ObjectInfo(string message, object obj, Func<bool>? condition = null);
        
        public void CollectionInfo(string message, IEnumerable collection, Func<bool>? condition = null);
        
        public void ObjectInfo<T>(
           string title,
           T obj,
           Action<T> unpack);

        public void CollectionInfo<T>(
              string title,
              IEnumerable<T> collection,
              Action<T> unpack);

        
        void Warning(string message);
        
        void Error(string message, Exception? ex = null);
        
        void Fatal(string message, Exception? ex = null);
    }
}