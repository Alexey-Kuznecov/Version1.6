using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media;
using UnityCommander.Logging.Core;
using UnityCommander.Modules.BottomPanel.Highlighting;

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
                new($"[{entry.Scope}] ", new HighlightStyle(Brushes.Gray)),
                new($"[{entry.Category}] ", new HighlightStyle(Brushes.DodgerBlue)),
                new($"[{entry.Level}] ", new HighlightStyle(Brushes.LightGray))
            };

            BuildMessage(entry.Message, list);

            if (entry.Payload != null)
            {
                list.Add(new($"[{entry.Payload}] ", new HighlightStyle(Brushes.RosyBrown)));
            }
            return list;
        }

        private static void BuildMessage(string text, List<LogInline> list)
        {
            int last = 0;

            foreach (Match m in PathRegex.Matches(text))
            {
                if (m.Index > last)
                    list.Add(new LogInline(text[last..m.Index], HighlightStyles.Default));

                list.Add(new LogInline(m.Value, new HighlightStyle(Brushes.DeepSkyBlue)));
                last = m.Index + m.Length;
            }

            if (last < text.Length)
                list.Add(new LogInline(text[last..], HighlightStyles.Default));
        }
    }
}
