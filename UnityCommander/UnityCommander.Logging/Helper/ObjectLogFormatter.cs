
namespace UnityCommander.Logging.Abstractions.Helper
{
    using System.Reflection;
    using System.Text;

    public static class ObjectLogFormatter
    {
        public static string Format(object? obj)
        {
            if (obj == null)
                return null;

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
    }
}
