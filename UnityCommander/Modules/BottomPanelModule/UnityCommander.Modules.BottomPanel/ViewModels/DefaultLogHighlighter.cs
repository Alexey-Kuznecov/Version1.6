
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media;
using UnityCommander.Logging.Abstractions;

namespace UnityCommander.Modules.BottomPanel.ViewModels
{
    public sealed class DefaultLogHighlighter : ILogHighlighter
    {
        private static readonly Regex PathRegex =
            new(@"\([A-Z]:\\[^\)]*\)", RegexOptions.Compiled);

        public IReadOnlyList<LogInline> Build(LogEntry entry)
        {
            var list = new List<LogInline>
            {
                new($"[{entry.Scope}] ", Brushes.Gray),
                new($"[{entry.Category}] ", Brushes.DodgerBlue),
                new($"[{entry.Level}] ", Brushes.LightGray)
            };

            BuildMessage(entry.Message, list);

            if (entry.Payload != null)
            {
                list.Add(new($"[{entry.Payload}] ", Brushes.RosyBrown));
            }
            return list;
        }

        private static void BuildMessage(string text, List<LogInline> list)
        {
            int last = 0;

            foreach (Match m in PathRegex.Matches(text))
            {
                if (m.Index > last)
                    list.Add(new LogInline(text[last..m.Index], Brushes.White));

                list.Add(new LogInline(m.Value, Brushes.DeepSkyBlue));
                last = m.Index + m.Length;
            }

            if (last < text.Length)
                list.Add(new LogInline(text[last..], Brushes.White));
        }
    }
}
