using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.CLI.Input
{
    public class InputContextResolver : IInputContextResolver
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
                var tokensResult = tokens;
                InputToken currentToken;

                if (tokensResult.Tokens.Count == 0)
                {
                    currentToken = new InputToken
                    {
                        Start = 0,
                        Length = 0,
                        Value = string.Empty
                    };
                }
                else
                {
                    currentToken = tokensResult.Tokens
                        .FirstOrDefault(t =>
                            tokensResult.CaretPosition >= t.Start &&
                            tokensResult.CaretPosition <= t.Start + t.Length)
                        ?? new InputToken
                        {
                            Start = tokensResult.CaretPosition,
                            Length = 0,
                            Value = string.Empty
                        };
                }

                return new ArgumentContext(
                    tokensResult,
                    commandName: tokensResult.Tokens.FirstOrDefault()?.Value ?? string.Empty,
                    existingArguments: tokensResult.Tokens.Skip(1).Select(t => t.Value ?? string.Empty).ToArray(),
                    partialArgument: currentToken.Value ?? throw new InvalidOperationException("currentToken не может быть null"),
                    replaceStart: currentToken.Start,
                    replaceLength: currentToken.Length);
            }
        }
    }
}
