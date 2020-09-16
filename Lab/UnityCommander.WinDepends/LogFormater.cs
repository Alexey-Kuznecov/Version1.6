using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.WinDepends
{
    public class LogFormater
    {
        public static string DrawHeader(string header)
        {
            byte count = 50;
            byte pos = (byte)((count - header.Length) / 2);
            string str = new string('_', pos);
            str += header;
            str += new string('_', pos);
            return str;
        }

        public static string DrawInfoHeader()
        {
            var frame = new StackFrame(1);
            string fullName = frame.GetMethod().ReflectedType.FullName;
            string methodName = frame.GetMethod().Name;
            string className = frame.GetMethod().DeclaringType.FullName;
            ParameterInfo[] parameters = frame.GetMethod().GetParameters();
            string param = "<No Parameters>";
            int max = Math.Max(Math.Max(param.Length, className.Length), methodName.Length) + 10;

            foreach (var item in parameters)
            {
                param += string.Format("{1} {0}, ", item.Name, item.ParameterType.Name);
            }

            string str = "+" + new string('-', max) + "+";
            str += "\n| Class:  " + className + new string(' ', (max - className.Length) - 9) + "|\n";
            str += "| Method: " + methodName + new string(' ', (max - methodName.Length) - 9) + "|\n";
            str += "| Param:  " + param + new string(' ', (max - param.Length) - 9) + "|\n";
            str += "+" + new string('-', max) + "+";

            return str;
        }

        static List<string> MemberColumn;
        static List<string> ModeColumn;
        static List<string> TypeColumn;
        static List<string> NameColumn;
        static List<string> HeaderColumn;
        static List<string> ValueColumn;
        private static List<Dictionary<string, string>> Table;
        private static byte maxLen;

        public static string DrawObject(object obj)
        {
            string[] headers = { "Member", "Mode", "Type", "Name", "Value" };

            Table = new List<Dictionary<string, string>>();
            PropertyInfo[] properties = obj.GetType().GetProperties();
            FieldInfo[] fileds = obj.GetType().GetFields();

            foreach (var filed in fileds)
            {
                var cell = new Dictionary<string, string>();
                cell.Add("Member", "Field");
                cell.Add("Mode", filed.IsPublic ? "+" : "-");
                cell.Add("Type", filed.FieldType.ToString());
                cell.Add("Name", filed.Name);
                cell.Add("Value", filed.GetValue(obj).ToString());

                Table.Add(cell);
            }

            var cell2 = new Dictionary<string, string>();
            
            for (int i = 0; i < headers.Length; i++)
            {
                foreach (var h in headers)
                {
                  //  headers[i]("Header", h);
                }                
            }

            MemberColumn = CreateColumn("Member");
            ModeColumn = CreateColumn("Mode");
            TypeColumn = CreateColumn("Type");
            NameColumn = CreateColumn("Name");
            ValueColumn = CreateColumn("Value");
            HeaderColumn = CreateColumn("Value");

            return MergeColumn();
        }

        public static string MergeColumn()
        {
            List<string> merge = new List<string>();
            string col = "";

            for (int i = 0; i < Table.Count; i++)
            {
                merge.Add(HeaderColumn[i] + MemberColumn[i] + ModeColumn[i] + TypeColumn[i] + NameColumn[i] + ValueColumn[i] + " |\n");
            }

            foreach (var item in merge)
            {
                col += item;
            }
            col.Replace('_', '\n');
            return col;
        }

        public static List<string> CreateColumn(string header)
        {
            var rows = new List<string>();

            foreach (var item in Table)
            {
                var val = item[header];
                rows.Add(val);
                    
                if (val.Length > maxLen)
                {
                    maxLen = (byte)val.Length;
                }
            }

            for (int i = 0; i < rows.Count; i++)
            {
                rows[i] = rows[i] + "".PadRight(maxLen - rows[i].Length);
            }

            for (int i = 0; i < rows.Count; i++)
            {
                rows[i] = "| " + rows[i];
            }

            return rows;
        }
    }
}
