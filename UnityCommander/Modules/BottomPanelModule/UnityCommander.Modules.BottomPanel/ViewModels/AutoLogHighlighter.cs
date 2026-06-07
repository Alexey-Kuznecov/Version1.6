using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media;
using UnityCommander.Logging.Core;
using UnityCommander.Modules.BottomPanel.Highlighting;

namespace UnityCommander.Modules.BottomPanel.ViewModels
{
    public sealed class AutoLogHighlighter : ILogHighlighter
    {
        private readonly ILogHighlightRule[] _rules =
        [
             new GuidHighlightRule(),
             new PathHighlightRule()
        ];

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
                list.Add(new($"[{entry.Payload}] ", 
                    new HighlightStyle(Brushes.RosyBrown)));
            }
            return list;
        }

        private void BuildMessage(string text, List<LogInline> list)
        {
            int position = 0;

            while (position < text.Length)
            {
                Match? bestMatch = null;
                ILogHighlightRule? bestRule = null;

                foreach (var rule in _rules)
                {
                    var match = rule.Pattern.Match(text, position);

                    if (!match.Success)
                        continue;

                    if (bestMatch == null || match.Index < bestMatch.Index)
                    {
                        bestMatch = match;
                        bestRule = rule;
                    }
                }

                if (bestMatch == null)
                {
                    list.Add(new LogInline(text[position..],
                        HighlightStyles.Default));
                    break;
                }

                if (bestMatch.Index > position)
                {
                    list.Add(new LogInline(
                        text[position..bestMatch.Index],
                        HighlightStyles.Default));
                }

                list.Add(new LogInline(
                    bestMatch.Value,
                    bestRule!.GetStyle(bestMatch.Value)));

                position = bestMatch.Index + bestMatch.Length;
            }
        }
    }
}
