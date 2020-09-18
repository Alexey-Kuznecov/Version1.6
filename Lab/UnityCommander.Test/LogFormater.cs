
namespace UnityCommander.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// The log formatter.
    /// </summary>
    public class LogFormatter
    {
        /// <summary>
        /// The headers arr.
        /// </summary>
        private static string[] HeaderNames;

        /// <summary>
        /// The member column.
        /// </summary>
        private static List<string> memberColumn;

        /// <summary>
        /// The mode column.
        /// </summary>
        private static List<string> modeColumn;

        /// <summary>
        /// The type column.
        /// </summary>
        private static List<string> typeColumn;

        /// <summary>
        /// The name column.
        /// </summary>
        private static List<string> nameColumn;

        /// <summary>
        /// The value column.
        /// </summary>
        private static List<string> valueColumn;

        /// <summary>
        /// The table.
        /// </summary>
        private static List<Dictionary<string, string>> table;

        /// <summary>
        /// The max len.
        /// </summary>
        private static byte maxLen;

        /// <summary>
        /// The max len.
        /// </summary>
        private static List<string> maxLenValue;

        /// <summary>
        /// The headers.
        /// </summary>
        private static string headers;

        /// <summary>
        /// The header separator.
        /// </summary>
        private static string headerSeparator;

        /// <summary>
        /// The border table.
        /// </summary>
        private static string borderTable;

        /// <summary>
        /// The columns width.
        /// </summary>
        private static List<int> columnsWidth = new List<int>();

        /// <summary>
        /// The draw header.
        /// </summary>
        /// <param name="header">
        /// The header.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string DrawHeader(string header)
        {
            byte count = 50;
            byte pos = (byte)((count - header.Length) / 2);
            string str = new string('_', pos);
            str += header;
            str += new string('_', pos);
            return str;
        }

        /// <summary>
        /// The draw info header.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string DrawInfoHeader()
        {
            var frame = new StackFrame(1);
            string methodName = frame.GetMethod().Name;
            string className = frame.GetMethod().DeclaringType?.FullName;
            ParameterInfo[] parameters = frame.GetMethod().GetParameters();
            string param = "";

            if (parameters.Length == 0)
            {
                param = "<No parameters>";
            }

            foreach (var item in parameters)
            {
                param += string.Format("{1} {0}, ", item.Name, item.ParameterType.Name);
            }

            int max = Math.Max(Math.Max(param.Length, className.Length), methodName.Length) + 10;
            string str = "+" + new string('-', max) + "+";
            str += "\n| Class:  " + className + new string(' ', (max - className.Length) - 9) + "|\n";
            str += "| Method: " + methodName + new string(' ', (max - methodName.Length) - 9) + "|\n";
            str += "| Param:  " + param + new string(' ', (max - param.Length) - 9) + "|\n";
            str += "+" + new string('-', max) + "+";
            return str;
        }

        /// <summary>
        /// The draw local vars.
        /// </summary>
        /// <param name="val">
        /// The val.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string DrawLocalVars(object[] val)
        {
            HeaderNames = new[] { "Type", "Name", "Value" };
            table = new List<Dictionary<string, string>>();
            StackFrame stackFrame = new StackFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();
            var className = stackFrame.GetMethod().DeclaringType;
            var variables = methodBase.GetMethodBody()?.LocalVariables;
            var sdas = className.GetMethod("<GetEnumerator>d__3");

            CreateHeader();
            return null;
        }

        /// <summary>
        /// The draw object.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string DrawObject(object obj)
        {
            HeaderNames = new[] { "Member", "Access", "Type", "Name", "Value" };
            columnsWidth = new List<int>();
            maxLenValue = new List<string>();
            table = new List<Dictionary<string, string>>();
            PropertyInfo[] properties = obj.GetType().GetProperties(
                BindingFlags.Instance
                        | BindingFlags.NonPublic
                        | BindingFlags.Public);
            FieldInfo[] fields = obj.GetType().GetFields(
                 BindingFlags.Instance
                         | BindingFlags.NonPublic
                         | BindingFlags.Public);

            // Initial column field data.
            foreach (var filed in fields)
            {
                if (filed.GetValue(obj) == null || filed.Name.Contains("<")) continue;
                var cell = new Dictionary<string, string>
                    {
                        { "Member", "Field" },
                        { "Access", filed.IsPublic ? "Public" : "Private" },
                        { "Type", filed.FieldType.ToString() },
                        { "Name", filed.Name },
                        { "Value", filed.GetValue(obj).ToString() }
                    };
                table.Add(cell);
            }

            // Initial column properties data.
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.GetValue(obj) == null) continue;
                var cell = new Dictionary<string, string>
                    {
                        { "Access", propertyInfo.PropertyType.IsPublic ? "Public" : "Private" },
                        { "Type", propertyInfo.PropertyType.ToString() },
                        { "Name", propertyInfo.Name },
                        { "Value", propertyInfo.GetValue(obj).ToString() },
                        { "Member", "Property" }
                    };
                table.Add(cell);
            }

            // Create columns,
            memberColumn = CreateColumn("Member");
            modeColumn = CreateColumn("Access");
            typeColumn = CreateColumn("Type");
            nameColumn = CreateColumn("Name");
            valueColumn = CreateColumn("Value");
            CreateHeader();
            return MergeColumn();
        }

        /// <summary>
        /// The create header.
        /// </summary>
        public static void CreateHeader()
        {
            headers = string.Empty;
            headerSeparator = string.Empty;
            borderTable = string.Empty;

            // Create the header and the table borders.
            for (var index = 0; index < HeaderNames.Length; index++)
            {
                var header = HeaderNames[index];
                var width = columnsWidth[index];
                var minus = width - header.Length;
                string space = new string(' ', minus);
                headerSeparator += new string('-', width + 2) + "+";
                borderTable += new string('-', width);
                headers += $"| {header}{space} ";
            }

            // Add a dash to reach the borders of the table.
            borderTable += new string('-', HeaderNames.Length + 9);
        }

        /// <summary>
        /// The merge column.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string MergeColumn()
        {
            List<string> merge = new List<string>();
            string columns = string.Empty;

            // Concat values into single line.
            for (int i = 0; i < table.Count; i++)
            {
                merge.Add($"{memberColumn[i]} {modeColumn[i]} {typeColumn[i]} {nameColumn[i]} {valueColumn[i]} |\n");
            }

            // Concat all lines into one.
            foreach (var item in merge)
            {
                columns += item;
            }

            // Concatenation the header and columns lines.
            return "+" + borderTable + "+\n"
                       + headers + "|" + "\n"
                       + "|" + headerSeparator + " \n"
                       + columns
                       + "+" + borderTable + "+";
        }

        /// <summary>
        /// The create column.
        /// </summary>
        /// <param name="header">
        /// The header.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<string> CreateColumn(string header)
        {
            var rows = new List<string>();
            string longValue = string.Empty;
            maxLen = 0;

            // Assignment values of the cells in the table.
            for (var index = 0; table.Count > index; index++)
            {
                var cell = table[index][header];

                // To capture the maximum value and to keep each columns width to array.
                if (cell.Length > maxLen)
                {
                    maxLen = (byte)cell.Length;
                    longValue = cell;
                }

                // Also given the width of the headers
                if (index >= HeaderNames.Length)
                    continue;

                // If the header value is larger than the column's cell,
                // when should the header size be used.
                var head = HeaderNames[index];
                maxLen = (byte)Math.Max(maxLen, head.Length);
            }

            // Keeping the most longer a values of the column.
            columnsWidth.Add(maxLen);
            maxLenValue.Add(longValue);

            // Assignment values of the cells in the table.
            foreach (var t in table)
            {
                var cell = t[header];
                rows.Add(cell);
            }

            // Creating a cell space.
            for (int i = 0; i < rows.Count; i++)
            {
                rows[i] = rows[i] + string.Empty.PadRight(maxLen - rows[i].Length);
            }

            // Adding a separator character to the table.
            for (int i = 0; i < rows.Count; i++)
            {
                rows[i] = "| " + rows[i];
            }

            return rows;
        }
    }
}
