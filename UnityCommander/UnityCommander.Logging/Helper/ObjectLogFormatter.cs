
namespace UnityCommander.Logging.Abstractions.Helper
{
    using System.Reflection;
    using System.Text;
    using System.Collections;

    public static class ObjectLogFormatter
    {
        public static string Format(object? obj)
        {
            if (obj == null)
                return null;

            // ❗ коллекции (но не string)
            if (obj is IEnumerable enumerable && obj is not string)
                return FormatEnumerable(enumerable);

            var type = obj.GetType();

            // Простые типы — логируем напрямую
            if (IsSimple(type))
                return obj.ToString()!;

            var sb = new StringBuilder();
            sb.Append(type.Name);
            sb.Append(" { ");

            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];

                if (!prop.CanRead)
                    continue;

                object? value;
                try
                {
                    value = prop.GetValue(obj);
                }
                catch
                {
                    value = "<error>";
                }

                sb.Append(prop.Name);
                sb.Append('=');
                sb.Append(FormatValue(value));

                if (i < props.Length - 1)
                    sb.Append(", ");
            }

            sb.Append(" }");
            return sb.ToString();
        }

        private static string FormatValue(object? value)
        {
            if (value == null)
                return "null";

            var type = value.GetType();

            if (IsSimple(type))
                return value.ToString()!;

            // Чтобы не уходить в рекурсию
            return $"<{type.Name}>";
        }

        private static bool IsSimple(Type type)
        {
            return type.IsPrimitive
                || type.IsEnum
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(Guid);
        }

        private static string FormatEnumerable(IEnumerable enumerable)
        {
            var sb = new StringBuilder();
            sb.Append('[');

            bool first = true;

            foreach (var item in enumerable)
            {
                if (!first)
                    sb.Append(", ");

                sb.Append(FormatItem(item));
                first = false;
            }

            sb.Append(']');
            return sb.ToString();
        }

        private static string FormatItem(object? item)
        {
            if (item == null)
                return "null";

            return item switch
            {
                string s => $"\"{s}\"",
                char c => $"'{c}'",
                _ => item.ToString() ?? item.GetType().Name
            };
        }
    }
}
