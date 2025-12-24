using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.CLI.Input
{
    public class SimpleInputContextResolver : IInputContextResolver
    {
        public InputContext Resolve(TokenizationResult tokens)
        {
            // Проверка на null для избежания CS8602
            var tokenList = tokens.Tokens ?? Array.Empty<InputToken>();

            // Простейшая логика: если токен первый → команда, иначе аргумент
            if (tokenList.Count == 0 || tokenList.Count == 1)
            {
                var partial = tokenList.FirstOrDefault()?.Value ?? "";
                return new CommandNameContext(tokens, partial);
            }
            else
            {
                var cmd = tokenList[0].Value;
                var args = tokenList.Skip(1).Select(t => t.Value ?? string.Empty).ToArray();
                var partialArg = args.Last();
                var lastToken = tokens.Tokens!.Last();

                var replaceStart = lastToken.StartIndex;
                var replaceLength = lastToken.Length;
                var activeToken = tokens;
                return new ArgumentContext(
                    tokens,
                    cmd ?? string.Empty,
                    args,
                    lastToken.Value ?? string.Empty,
                    replaceStart,
                    replaceLength
                );
            }
        }
    }
}
