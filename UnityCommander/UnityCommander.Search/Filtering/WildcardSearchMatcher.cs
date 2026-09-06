
using UnityCommander.Search.Abstractions;
using UnityCommander.Search.Models;

namespace UnityCommander.Search.Filtering
{
    public sealed class WildcardSearchMatcher : ISearchMatcher
    {
        public bool Match(SearchItem item, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return true;

            if (item.Name is null)
                return false;

            return Match(item.Name, query);
        }

        private static bool Match(string text, string pattern)
        {
            var textIndex = 0;
            var patternIndex = 0;

            var starIndex = -1;
            var matchIndex = 0;

            while (textIndex < text.Length)
            {
                if (patternIndex < pattern.Length &&
                    (pattern[patternIndex] == '?' ||
                     char.ToUpperInvariant(pattern[patternIndex]) ==
                     char.ToUpperInvariant(text[textIndex])))
                {
                    textIndex++;
                    patternIndex++;
                }
                else if (patternIndex < pattern.Length &&
                         pattern[patternIndex] == '*')
                {
                    starIndex = patternIndex++;
                    matchIndex = textIndex;
                }
                else if (starIndex != -1)
                {
                    patternIndex = starIndex + 1;
                    textIndex = ++matchIndex;
                }
                else
                {
                    return false;
                }
            }

            while (patternIndex < pattern.Length &&
                   pattern[patternIndex] == '*')
            {
                patternIndex++;
            }

            return patternIndex == pattern.Length;
        }
    }
}
