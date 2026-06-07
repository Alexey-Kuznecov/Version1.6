
using UnityCommander.Logging.Contracts;

namespace UnityCommander.Logging.Extensions
{
    public static class LoggerExtensions
    {
        public static void ObjectDiff<T>(
            this ILogger logger,
            string title,
            T before,
            T after,
            Action<T, T> compare)
        {
            logger.Info(title);

            compare(before, after);

            logger.Info("--------------------");
        }

        public static void CollectionDiff<T>(
           this ILogger logger,
           string title,
           IEnumerable<T> before,
           IEnumerable<T> after,
           Action<ILogger, T, T> compare)
        {
            logger.Info(title);

            using var left = before.GetEnumerator();
            using var right = after.GetEnumerator();

            int index = 0;

            while (left.MoveNext() && right.MoveNext())
            {
                logger.Info($"[{index}]");

                compare(logger, left.Current, right.Current);

                index++;
            }
        }
    }
}
