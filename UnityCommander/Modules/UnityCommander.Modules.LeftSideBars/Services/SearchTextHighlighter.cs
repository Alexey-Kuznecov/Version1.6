
using System;
using System.Collections.Generic;
using UnityCommander.Modules.LeftSideBars.Models;

namespace UnityCommander.Modules.LeftSideBars.Services
{
    public static class SearchTextHighlighter
    {
        public static IReadOnlyList<SearchTextPart> Create(
            string text,
            string query)
        {
            if (string.IsNullOrEmpty(text) ||
                string.IsNullOrEmpty(query))
            {
                return [new SearchTextPart(text, false)];
            }

            var result = new List<SearchTextPart>();
            var position = 0;

            while (true)
            {
                var index = text.IndexOf(
                    query,
                    position,
                    StringComparison.OrdinalIgnoreCase);

                if (index < 0)
                    break;

                if (index > position)
                {
                    result.Add(
                        new SearchTextPart(
                            text[position..index],
                            false));
                }

                result.Add(
                    new SearchTextPart(
                        text.Substring(index, query.Length),
                        true));

                position = index + query.Length;
            }

            if (position < text.Length)
            {
                result.Add(
                    new SearchTextPart(
                        text[position..],
                        false));
            }

            return result;
        }
    }
}
